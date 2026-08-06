using System.Text.Json;

namespace FdFinance.Domain.Common;

/// <summary>
/// 原表实体基类。
/// ExtensionJson 为<strong>可空附加列</strong>（新系统可用；旧系统不写即可继续跑）。
/// 禁止改名/改类型/删除原有 F_* 字段。
/// </summary>
public abstract class ExtensibleEntity
{
    /// <summary>
    /// 可空 JSON 扩展列。旧库无此列时：部署时执行 ADD NULL 脚本；旧程序 SELECT/INSERT 不涉及该列仍可运行。
    /// </summary>
    public string? ExtensionJson { get; set; }

    public T? GetExtension<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(ExtensionJson)) return default;
        try
        {
            using var doc = JsonDocument.Parse(ExtensionJson);
            if (doc.RootElement.TryGetProperty(key, out var el))
                return el.Deserialize<T>();
        }
        catch { /* ignore */ }
        return default;
    }

    public void SetExtension<T>(string key, T value)
    {
        var dict = string.IsNullOrWhiteSpace(ExtensionJson)
            ? new Dictionary<string, object?>()
            : JsonSerializer.Deserialize<Dictionary<string, object?>>(ExtensionJson)
              ?? new Dictionary<string, object?>();
        dict[key] = value;
        ExtensionJson = JsonSerializer.Serialize(dict);
    }
}
