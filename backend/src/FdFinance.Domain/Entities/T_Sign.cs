using FdFinance.Domain.Common;

namespace FdFinance.Domain.Entities;

/// <summary>Original table: T_Sign — approval chain steps.</summary>
public class T_Sign : ExtensibleEntity
{
    public Guid F_SignId { get; set; }
    public DateTime F_CreateDate { get; set; }
    public Guid F_ReimbursementId { get; set; }
    public Guid F_AbstractId { get; set; }
    /// <summary>0=waiting, 1=approved, 2=current pending (legacy).</summary>
    public int F_IsN { get; set; }
    public DateTime? F_SignDate { get; set; }
    public string? F_Position { get; set; }
    public string? F_Name { get; set; }
    public int F_Sort { get; set; }
    public string? F_ImageUrl { get; set; }
    public string? F_Status { get; set; }
    public string? TodoId { get; set; }

    public T_Reimbursement? Reimbursement { get; set; }
    public T_Abstract? Abstract { get; set; }
}
