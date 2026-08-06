using FdFinance.Domain.Common;

namespace FdFinance.Domain.Entities;

/// <summary>Original table: T_Relation — user ↔ autograph mapping.</summary>
public class T_Relation : ExtensibleEntity
{
    public Guid F_RelationId { get; set; }
    public Guid F_UserId { get; set; }
    public Guid F_AutographId { get; set; }
}
