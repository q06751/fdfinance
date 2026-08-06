using FdFinance.Domain.Common;

namespace FdFinance.Domain.Entities;

/// <summary>
/// 原表 T_Admin（保留 F_LoginNmae）。
/// F_IsUP 为旧权限字段；F_Role 为可空附加列（无列时仅用 F_IsUP 推断）。
/// </summary>
public class T_Admin : ExtensibleEntity
{
    public string F_AdminId { get; set; } = string.Empty;
    public string? F_LoginNmae { get; set; }
    public string? F_PassWord { get; set; }
    /// <summary>旧字段：1=管理员。</summary>
    public int F_IsUP { get; set; }
    public int F_Depid { get; set; }
    public string? F_DName { get; set; }
    /// <summary>可空附加：admin / user / approver。旧系统不读不影响。</summary>
    public string? F_Role { get; set; }

    /// <summary>登录角色：优先 F_Role，否则 F_IsUP=1 → admin。</summary>
    public string ResolveRole()
    {
        if (!string.IsNullOrWhiteSpace(F_Role))
            return F_Role.Trim().ToLowerInvariant();
        return F_IsUP == 1 ? "admin" : "user";
    }
}
