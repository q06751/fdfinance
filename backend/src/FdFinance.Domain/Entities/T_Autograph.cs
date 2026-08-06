using FdFinance.Domain.Common;

namespace FdFinance.Domain.Entities;

/// <summary>Original table: T_Autograph — signature image assets.</summary>
public class T_Autograph : ExtensibleEntity
{
    public Guid F_AutographId { get; set; }
    public string? F_Url { get; set; }
    public Guid F_UserId { get; set; }
}
