namespace FdFinance.Domain.Enums;

/// <summary>
/// Derived workflow status from original F_IsSend / F_IsStatus / sign counts.
/// Original columns remain the source of truth.
/// </summary>
public enum ReimbursementWorkflowStatus
{
    Draft = 0,       // F_IsSend=0, F_IsStatus=0
    InApproval = 1,  // F_IsSend=1, partial signs
    Approved = 2,    // F_IsSend=1, all signs done
    Voided = 3       // F_IsStatus=1
}

/// <summary>
/// T_Sign.F_IsN values from the legacy system.
/// </summary>
public static class SignIsN
{
    public const int Waiting = 0;
    public const int Approved = 1;
    public const int PendingCurrent = 2;
}

/// <summary>
/// T_Sign.F_Status letter codes from the legacy mobile approval module.
/// A=未审批, D=审批中(我已签整单未完), B=已审批(整单完成), C=已作废.
/// </summary>
public static class SignStatus
{
    /// <summary>待我处理（未审批）— weichuli.aspx</summary>
    public const string Pending = "A";
    /// <summary>整单全部签完 — yishenpi.aspx</summary>
    public const string Completed = "B";
    /// <summary>关联作废 — yizuofei.aspx</summary>
    public const string Voided = "C";
    /// <summary>我已签、整单还在流转 — shenpizhong.aspx</summary>
    public const string SignedInProgress = "D";
}
