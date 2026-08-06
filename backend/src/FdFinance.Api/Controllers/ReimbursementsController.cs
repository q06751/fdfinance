using System.Security.Claims;
using FdFinance.Application.Common;
using FdFinance.Application.DTOs;
using FdFinance.Application.Interfaces;
using FdFinance.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FdFinance.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ReimbursementsController : ControllerBase
{
    private readonly IReimbursementService _svc;
    private readonly IAuthService _auth;

    public ReimbursementsController(IReimbursementService svc, IAuthService auth)
    {
        _svc = svc;
        _auth = auth;
    }

    /// <summary>列表。typt：1 报销 2 付款 3 收款；不传默认 1；传 0 表示全部。</summary>
    [HttpGet]
    public async Task<ActionResult> List(
        [FromQuery] string? keyword,
        [FromQuery] string? status,
        [FromQuery] int? category,
        [FromQuery] int? typt = 1,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        int? filter = typt is null or 1 or 2 or 3 ? typt : null;
        if (typt == 0) filter = null;
        var result = await _svc.ListAsync(keyword, status, category, page, pageSize, CurrentActor(), filter, ct);
        return Ok(ApiResult<PagedResult<ReimbursementListItemDto>>.Ok(result));
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult> Dashboard(CancellationToken ct)
        => Ok(ApiResult<DashboardStatsDto>.Ok(await _svc.GetDashboardAsync(CurrentActor(), ct)));

    [HttpGet("approvals")]
    public async Task<ActionResult> Approvals([FromQuery] string tab = "pending", CancellationToken ct = default)
        => Ok(ApiResult<IReadOnlyList<ApprovalTaskDto>>.Ok(
            await _svc.ListApprovalsAsync(CurrentActor(), tab, ct)));

    [HttpGet("approvals/counts")]
    public async Task<ActionResult> ApprovalCounts(CancellationToken ct)
        => Ok(ApiResult<ApprovalCountsDto>.Ok(await _svc.GetApprovalCountsAsync(CurrentActor(), ct)));

    [HttpGet("pending-approvals")]
    public async Task<ActionResult> PendingApprovals(CancellationToken ct)
        => Ok(ApiResult<IReadOnlyList<ApprovalTaskDto>>.Ok(
            await _svc.ListApprovalsAsync(CurrentActor(), "pending", ct)));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> Get(Guid id, CancellationToken ct)
    {
        var detail = await _svc.GetAsync(id, CurrentActor(), ct);
        if (detail is null) return NotFound(ApiResult<object>.Fail("单据不存在或无权查看"));
        return Ok(ApiResult<ReimbursementDetailDto>.Ok(detail));
    }

    [HttpGet("{id:guid}/approval-workspace")]
    public async Task<ActionResult> Workspace(Guid id, CancellationToken ct)
    {
        var ws = await _svc.GetApprovalWorkspaceAsync(id, CurrentActor(), ct);
        if (ws is null) return NotFound(ApiResult<object>.Fail("暂无待签内容或无权查看"));
        return Ok(ApiResult<ApprovalWorkspaceDto>.Ok(ws));
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateReimbursementRequest req, CancellationToken ct)
    {
        var admin = await CurrentAdmin(ct);
        if (admin is null) return Unauthorized();
        var result = await _svc.CreateAsync(req, admin, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateReimbursementRequest req, CancellationToken ct)
    {
        var result = await _svc.UpdateAsync(id, req, CurrentActor(), ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _svc.SoftDeleteAsync(id, CurrentActor(), ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:guid}/void")]
    public async Task<ActionResult> Void(Guid id, CancellationToken ct)
    {
        var result = await _svc.VoidAsync(id, CurrentActor(), ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:guid}/approvers")]
    public async Task<ActionResult> AddApprover(Guid id, [FromBody] AddApproverRequest req, CancellationToken ct)
    {
        var result = await _svc.AddApproverAsync(id, req, CurrentActor(), ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}/approver-chain")]
    public async Task<ActionResult> SetApproverChain(Guid id, [FromBody] SetApproverChainRequest req, CancellationToken ct)
    {
        var result = await _svc.SetApproverChainAsync(id, req, CurrentActor(), ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:guid}/approvers")]
    public async Task<ActionResult> ClearApprovers(Guid id, CancellationToken ct)
    {
        var result = await _svc.ClearApproversAsync(id, CurrentActor(), ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<ActionResult> Submit(Guid id, CancellationToken ct)
    {
        var result = await _svc.SubmitAsync(id, CurrentActor(), ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<ActionResult> ApproveDoc(Guid id, [FromBody] ApproveSignRequest req, CancellationToken ct)
    {
        var result = await _svc.ApproveAsync(id, CurrentActor(), req, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:guid}/print")]
    public async Task<ActionResult> RecordPrint(Guid id, CancellationToken ct)
    {
        var result = await _svc.RecordPrintAsync(id, CurrentActor(), ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("signs/{signId:guid}/approve")]
    public async Task<ActionResult> ApproveOne(Guid signId, [FromBody] ApproveSignRequest? req, CancellationToken ct)
    {
        var result = await _svc.ApproveOneAsync(signId, CurrentActor(), req, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("merge")]
    public async Task<ActionResult> Merge([FromBody] MergeReimbursementsRequest req, CancellationToken ct)
    {
        var result = await _svc.MergeAsync(req, CurrentActor(), ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:guid}/split")]
    public async Task<ActionResult> Split(Guid id, CancellationToken ct)
    {
        var result = await _svc.SplitAsync(id, CurrentActor(), ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<ActionResult> Reject(Guid id, [FromBody] RejectRequest? req, CancellationToken ct)
    {
        var result = await _svc.RejectAsync(id, CurrentActor(), req ?? new RejectRequest(), ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private ActorContext CurrentActor()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var name = User.FindFirstValue(ClaimTypes.Name) ?? "";
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "user";
        var depId = int.TryParse(User.FindFirstValue("depid"), out var d) ? d : 0;
        return new ActorContext(id, name, depId, role);
    }

    private async Task<T_Admin?> CurrentAdmin(CancellationToken ct)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id)) return null;
        return await _auth.GetAdminAsync(id, ct);
    }
}
