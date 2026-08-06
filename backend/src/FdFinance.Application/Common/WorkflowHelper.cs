using FdFinance.Domain.Entities;
using FdFinance.Domain.Enums;

namespace FdFinance.Application.Common;

public static class WorkflowHelper
{
    public static ReimbursementWorkflowStatus Resolve(T_Reimbursement r, int totalSigns, int approvedSigns)
    {
        if (r.F_IsStatus == 1) return ReimbursementWorkflowStatus.Voided;
        if (r.F_IsSend != 1) return ReimbursementWorkflowStatus.Draft;
        if (totalSigns > 0 && totalSigns == approvedSigns) return ReimbursementWorkflowStatus.Approved;
        return ReimbursementWorkflowStatus.InApproval;
    }

    public static string Label(ReimbursementWorkflowStatus s) => s switch
    {
        ReimbursementWorkflowStatus.Draft => "待提交",
        ReimbursementWorkflowStatus.InApproval => "审批中",
        ReimbursementWorkflowStatus.Approved => "已完成",
        ReimbursementWorkflowStatus.Voided => "已作废",
        _ => "未知"
    };

    public static string ToKey(ReimbursementWorkflowStatus s) => s.ToString().ToLowerInvariant();
}
