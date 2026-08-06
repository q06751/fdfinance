namespace FdFinance.Application.DTOs;

public record LoginRequest(string LoginName, string Password);
public record LoginResponse(
    string Token,
    string AdminId,
    string LoginName,
    string DepartmentName,
    int DepId,
    string Role);

public record ApiResult<T>(bool Success, string? Message, T? Data)
{
    public static ApiResult<T> Ok(T data, string? message = null) => new(true, message, data);
    public static ApiResult<T> Fail(string message) => new(false, message, default);
}

public record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);
