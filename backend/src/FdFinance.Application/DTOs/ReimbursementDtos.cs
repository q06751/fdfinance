namespace FdFinance.Application.DTOs;

/// <summary>单据类型：1=报销 2=付款 3=收款（对齐旧 F_Typt）</summary>
public static class DocTypt
{
    public const int Reimbursement = 1;
    public const int Pay = 2;
    public const int Put = 3;

    public static string Label(int typt) => typt switch
    {
        Pay => "付款单",
        Put => "收款单",
        _ => "报销单"
    };
}

public record AbstractLineDto(
    string Abstract,
    decimal Money,
    int Num,
    int Sort,
    int DepId,
    string? ExtensionJson = null);

public record CreateReimbursementRequest(
    string Name,
    string DepartmentName,
    string Money,
    string? Des,
    string? DepartmentLeader,
    DateTime CreateDate,
    int Category,
    IReadOnlyList<AbstractLineDto> Abstracts,
    string? ExtensionJson = null,
    int Typt = 1);

public record UpdateReimbursementRequest(
    string Name,
    string DepartmentName,
    string Money,
    string? Des,
    string? DepartmentLeader,
    DateTime CreateDate,
    int Category,
    IReadOnlyList<AbstractLineDto> Abstracts,
    string? ExtensionJson = null);

public record AddApproverRequest(
    string ApproverName,
    string? Position = null);

/// <summary>审批链节点（岗位 + 人），顺序即 F_Sort。</summary>
public record ApproverStepDto(string ApproverName, string? Position = null);

public record SetApproverChainRequest(IReadOnlyList<ApproverStepDto> Steps);

public record ApproveSignRequest(
    string? Comment = null,
    string? ImageUrl = null,
    IReadOnlyList<Guid>? SignIds = null);

public record MergeReimbursementsRequest(IReadOnlyList<Guid> Ids);

public record RejectRequest(string? Comment = null, IReadOnlyList<Guid>? SignIds = null);

public record ReimbursementListItemDto(
    Guid Id,
    string? Code,
    string? Name,
    string? Producer,
    string? Money,
    string? DepartmentName,
    DateTime CreateDate,
    DateTime AddDate,
    int? IsSend,
    int? IsStatus,
    string WorkflowStatus,
    string WorkflowStatusLabel,
    int Category,
    string? ClassCode,
    string? ExtensionJson,
    int Show = 1,
    string? MergeId = null,
    int Typt = 1,
    string? TyptLabel = null);

public record SignDto(
    Guid SignId,
    Guid AbstractId,
    string? Name,
    string? Position,
    int IsN,
    int Sort,
    DateTime? SignDate,
    string? Status,
    string? ImageUrl);

public record AbstractDto(
    Guid AbstractId,
    string? Abstract,
    decimal Money,
    int Num,
    int Sort,
    int DepId,
    IReadOnlyList<SignDto> Signs,
    string? ExtensionJson);

public record ReimbursementDetailDto(
    Guid Id,
    string? Code,
    string? Name,
    string? Producer,
    string? Money,
    string? DepartmentName,
    string? DepartmentLeader,
    string? Des,
    DateTime CreateDate,
    DateTime AddDate,
    int? IsSend,
    int? IsStatus,
    int DepId,
    int Category,
    int Typt,
    int Show,
    string? ClassCode,
    string? MergeId,
    string WorkflowStatus,
    string WorkflowStatusLabel,
    IReadOnlyList<AbstractDto> Abstracts,
    IReadOnlyList<SignDto> AllSigns,
    string? ExtensionJson,
    IReadOnlyList<Guid>? MergedIds = null,
    int PrintCount = 0,
    string? TyptLabel = null);

public record PendingLineDto(
    Guid SignId,
    Guid AbstractId,
    string? Abstract,
    decimal Money,
    int Num,
    int Sort,
    string? Position,
    string? ApproverName);

public record ApprovalWorkspaceDto(
    Guid ReimbursementId,
    string? Code,
    string? Applicant,
    string? Producer,
    string? DepartmentName,
    string? Money,
    DateTime CreateDate,
    IReadOnlyList<PendingLineDto> PendingLines,
    IReadOnlyList<AbstractDto> AllAbstracts,
    string? MergeId,
    IReadOnlyList<Guid>? MergedIds);

public record ApprovalTaskDto(
    Guid SignId,
    Guid ReimbursementId,
    string? Code,
    string? Applicant,
    string? Producer,
    string? DepartmentName,
    string? Money,
    string? ApproverName,
    string? Position,
    int Sort,
    int IsN,
    string? SignStatus,
    string SignStatusLabel,
    DateTime CreateDate,
    DateTime AddDate,
    DateTime? SignDate,
    string? ClassCode,
    int Typt = 1,
    string? TyptLabel = null);

public record ApprovalCountsDto(
    int Pending,
    int InProgress,
    int Done,
    int Voided);

public record DashboardStatsDto(
    int Total,
    int Draft,
    int InApproval,
    int Approved,
    int Voided,
    decimal TotalAmount,
    IReadOnlyList<MonthlyAmountDto> MonthlyTrend,
    IReadOnlyList<CategoryAmountDto> ByCategory,
    int ReimbursementCount = 0,
    int PayCount = 0,
    int PutCount = 0);

public record MonthlyAmountDto(string Month, decimal Amount, int Count);
public record CategoryAmountDto(string Category, decimal Amount, int Count);

public record CategoryDto(int Id, string? Code, string? Name, DateTime CreateTime);
public record UserDto(Guid UserId, string? Name, string? Phone, string? Code);
public record DepartmentDto(int DepId, string? DName, string? ClassCode);
public record ApplyDto(
    Guid ApplyId,
    string? Name,
    string? Department,
    string? Money,
    string? Descripion,
    DateTime CreateDate);
public record CreateApplyRequest(
    string Name,
    string Department,
    string Money,
    string? Descripion);
