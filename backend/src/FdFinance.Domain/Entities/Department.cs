using FdFinance.Domain.Common;

namespace FdFinance.Domain.Entities;

/// <summary>
/// Org unit from DingTalk DB (department). Simplified for self-contained deploy.
/// Original keys: depID, dName.
/// </summary>
public class Department : ExtensibleEntity
{
    public int DepId { get; set; }
    public string? DName { get; set; }
    /// <summary>Serial prefix: JT / WL / ZD / FD / JN ...</summary>
    public string? ClassCode { get; set; }
    public int ParentId { get; set; }
    public int IsDelete { get; set; }
}
