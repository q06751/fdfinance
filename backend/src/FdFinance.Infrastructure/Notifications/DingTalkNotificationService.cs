using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FdFinance.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FdFinance.Infrastructure.Notifications;

/// <summary>
/// 钉钉通知（对齐旧 CreateTodo / send 文本）。
/// - Webhook 群机器人：配置 DingTalk:Webhook + Enabled=true 即可发送
/// - 未配置时仅记日志，业务不中断
/// - 待办 TodoId 本地生成写入 T_Sign.TodoId，Done 时再推一条完成消息
/// </summary>
public class DingTalkNotificationService : INotificationService
{
    private readonly ILogger<DingTalkNotificationService> _log;
    private readonly string _webhook;
    private readonly string _appBaseUrl;
    private readonly string _secret;
    private readonly bool _enabled;
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(8) };

    public DingTalkNotificationService(
        IConfiguration config,
        ILogger<DingTalkNotificationService> log)
    {
        _log = log;
        _webhook = (config["DingTalk:Webhook"] ?? "").Trim();
        _secret = (config["DingTalk:Secret"] ?? "").Trim();
        _appBaseUrl = (config["DingTalk:AppBaseUrl"] ?? config["App:PublicUrl"] ?? "").Trim().TrimEnd('/');
        _enabled = config.GetValue("DingTalk:Enabled", false)
                   && !string.IsNullOrWhiteSpace(_webhook);
    }

    public bool IsEnabled => _enabled;

    public async Task SendTextAsync(string userCodeOrName, string message, CancellationToken ct = default)
    {
        _log.LogInformation("[Notify] enabled={En} to={User} msg={Msg}", _enabled, userCodeOrName, message);
        if (!_enabled) return;
        await PostRobotAsync(
            $"【{userCodeOrName}】{message}",
            ct);
    }

    public async Task<string?> CreateTodoAsync(
        string userCodeOrName,
        string title,
        string description,
        string url,
        CancellationToken ct = default)
    {
        var todoId = Guid.NewGuid().ToString("N");
        var link = string.IsNullOrEmpty(_appBaseUrl)
            ? url
            : (url.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? url
                : $"{_appBaseUrl}{url}");

        _log.LogInformation(
            "[TodoCreate] id={Id} user={User} title={Title} url={Url}",
            todoId, userCodeOrName, title, link);

        var text =
            $"待办 · {title}\n" +
            $"处理人：{userCodeOrName}\n" +
            $"{description}\n" +
            $"打开：{link}\n" +
            $"(todo:{todoId})";

        if (_enabled)
            await PostRobotAsync(text, ct);

        return todoId;
    }

    public async Task DoneTodoAsync(string userCodeOrName, string todoId, CancellationToken ct = default)
    {
        _log.LogInformation("[TodoDone] id={Id} user={User}", todoId, userCodeOrName);
        if (!_enabled) return;
        await PostRobotAsync(
            $"已办结 · {userCodeOrName}\n(todo:{todoId})",
            ct);
    }

    private async Task PostRobotAsync(string content, CancellationToken ct)
    {
        try
        {
            var url = BuildSignedWebhook();
            // 群机器人 text + markdown 双兼容：先 text
            var payload = new Dictionary<string, object?>
            {
                ["msgtype"] = "text",
                ["text"] = new { content }
            };

            var res = await Http.PostAsJsonAsync(url, payload, ct);
            if (!res.IsSuccessStatusCode)
            {
                var body = await res.Content.ReadAsStringAsync(ct);
                _log.LogWarning("钉钉 Webhook HTTP {Code}: {Body}", (int)res.StatusCode, body);
                return;
            }

            // 部分机器人返回 errcode
            try
            {
                await using var stream = await res.Content.ReadAsStreamAsync(ct);
                using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
                if (doc.RootElement.TryGetProperty("errcode", out var code) && code.GetInt32() != 0)
                {
                    var errmsg = doc.RootElement.TryGetProperty("errmsg", out var m) ? m.GetString() : "";
                    _log.LogWarning("钉钉返回错误 {Code} {Msg}", code.GetInt32(), errmsg);
                }
            }
            catch
            {
                /* ignore parse */
            }
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "钉钉消息发送失败");
        }
    }

    /// <summary>加签机器人：secret 非空时按钉钉文档拼接 timestamp+sign。</summary>
    private string BuildSignedWebhook()
    {
        if (string.IsNullOrEmpty(_secret))
            return _webhook;

        var ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var stringToSign = $"{ts}\n{_secret}";
        using var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(_secret));
        var sign = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
        var sep = _webhook.Contains('?') ? "&" : "?";
        return $"{_webhook}{sep}timestamp={ts}&sign={Uri.EscapeDataString(sign)}";
    }
}
