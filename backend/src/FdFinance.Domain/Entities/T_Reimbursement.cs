using FdFinance.Domain.Common;

namespace FdFinance.Domain.Entities;

/// <summary>
/// Original table: T_Reimbursement (primary key F_ReimbursementId).
/// Column names and semantics preserved from Finance.Model.T_Reimbursement.
/// </summary>
public class T_Reimbursement : ExtensibleEntity
{
    public Guid F_ReimbursementId { get; set; }
    public string? F_DepartmentName { get; set; }
    public string? F_Name { get; set; }
    public string? F_Money { get; set; }
    public string? F_Code { get; set; }
    public DateTime F_CreateDate { get; set; }
    public int F_IsDelete { get; set; }
    public int? F_IsSend { get; set; }
    public int? F_IsStatus { get; set; }
    public string? F_Des { get; set; }
    public string? F_DepartmentLeader { get; set; }
    public int F_Depid { get; set; }
    public string? F_MergeId { get; set; }
    public string? F_Producer { get; set; }
    /// <summary>Legacy typo preserved: F_Typt (was F_type).</summary>
    public int F_Typt { get; set; }
    public int F_Show { get; set; }
    /// <summary>Legacy typo preserved: F_ClassTypt (was F_ClassType).</summary>
    public string? F_ClassTypt { get; set; }
    public DateTime F_AddDate { get; set; }
    public int F_Category { get; set; }

    public ICollection<T_Abstract> Abstracts { get; set; } = new List<T_Abstract>();
    public ICollection<T_Sign> Signs { get; set; } = new List<T_Sign>();
}
