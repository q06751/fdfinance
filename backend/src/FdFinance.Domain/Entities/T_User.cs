using FdFinance.Domain.Common;

namespace FdFinance.Domain.Entities;

/// <summary>Original table: T_User — approvers / employees in finance DB.</summary>
public class T_User : ExtensibleEntity
{
    public Guid F_UserId { get; set; }
    public string? F_Name { get; set; }
    public string? F_Phone { get; set; }
    public string? F_Code { get; set; }
    public int F_IsDelete { get; set; }
}
