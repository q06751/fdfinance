using FdFinance.Domain.Common;

namespace FdFinance.Domain.Entities;

/// <summary>Original table: T_Category — expense categories by org code.</summary>
public class T_Category : ExtensibleEntity
{
    public int F_Id { get; set; }
    public string? F_Code { get; set; }
    public string? F_Name { get; set; }
    public DateTime F_CreateTime { get; set; }
    public bool F_Isdelete { get; set; }
}
