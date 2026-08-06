namespace FdFinance.Application.Common;

/// <summary>Current authenticated operator, derived from JWT claims.</summary>
public sealed record ActorContext(
    string AdminId,
    string LoginName,
    int DepId,
    string Role)
{
    public bool IsAdmin =>
        string.Equals(Role, "admin", StringComparison.OrdinalIgnoreCase)
        || string.Equals(AdminId, "admin", StringComparison.OrdinalIgnoreCase);
}
