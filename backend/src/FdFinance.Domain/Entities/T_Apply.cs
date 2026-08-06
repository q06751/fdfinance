using FdFinance.Domain.Common;

namespace FdFinance.Domain.Entities;

/// <summary>Original table: T_Apply — payment/put-money applications.</summary>
public class T_Apply : ExtensibleEntity
{
    public Guid F_ApplyId { get; set; }
    public string? F_Name { get; set; }
    public string? F_Department { get; set; }
    public string? F_Money { get; set; }
    public string? F_Descripion { get; set; }
    public DateTime F_CreateDate { get; set; }
    public int F_IsDelete { get; set; }
}
