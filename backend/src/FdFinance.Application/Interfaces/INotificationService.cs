namespace FdFinance.Application.Interfaces;

/// <summary>
/// 钉钉/企业消息通知（未配置时 no-op，与旧系统 CreateTodo / send 对齐）。
/// </summary>
public interface INotificationService
{
    bool IsEnabled { get; }

    Task SendTextAsync(string userCodeOrName, string message, CancellationToken ct = default);

    Task<string?> CreateTodoAsync(
        string userCodeOrName,
        string title,
        string description,
        string url,
        CancellationToken ct = default);

    Task DoneTodoAsync(string userCodeOrName, string todoId, CancellationToken ct = default);
}
