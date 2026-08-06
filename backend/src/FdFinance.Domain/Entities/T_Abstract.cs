using FdFinance.Domain.Common;

namespace FdFinance.Domain.Entities;

/// <summary>Original table: T_Abstract — reimbursement line items (摘要).</summary>
public class T_Abstract : ExtensibleEntity
{
    public Guid F_AbstractId { get; set; }
    public string? F_Abstract { get; set; }
    public decimal F_Money { get; set; }
    public int F_Isdelete { get; set; }
    public Guid F_ReimbursementId { get; set; }
    public int F_Sort { get; set; }
    public int F_Depid { get; set; }
    public int F_Num { get; set; }

    public T_Reimbursement? Reimbursement { get; set; }
    public ICollection<T_Sign> Signs { get; set; } = new List<T_Sign>();
}
