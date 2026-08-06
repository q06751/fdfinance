using FdFinance.Application.Common;
using FdFinance.Application.DTOs;
using FdFinance.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FdFinance.Api.Controllers;

/// <summary>
/// 管理后台：账号、部门、审批人、费用类别等（仅 admin 角色）。
/// </summary>
[ApiController]
[Authorize(Roles = "admin")]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _svc;

    public AdminController(IAdminService svc) => _svc = svc;

    [HttpGet("overview")]
    public async Task<ActionResult> Overview(CancellationToken ct)
        => Ok(ApiResult<AdminOverviewDto>.Ok(await _svc.GetOverviewAsync(ct)));

    [HttpGet("accounts")]
    public async Task<ActionResult> Accounts(CancellationToken ct)
        => Ok(ApiResult<object>.Ok(await _svc.ListAccountsAsync(ct)));

    [HttpPost("accounts")]
    public async Task<ActionResult> CreateAccount([FromBody] CreateAdminRequest req, CancellationToken ct)
    {
        var r = await _svc.CreateAccountAsync(req, ct);
        return r.Success ? Ok(r) : BadRequest(r);
    }

    [HttpPut("accounts/{adminId}")]
    public async Task<ActionResult> UpdateAccount(string adminId, [FromBody] UpdateAdminRequest req, CancellationToken ct)
    {
        var r = await _svc.UpdateAccountAsync(adminId, req, ct);
        return r.Success ? Ok(r) : BadRequest(r);
    }

    [HttpPost("accounts/{adminId}/reset-password")]
    public async Task<ActionResult> ResetPassword(string adminId, [FromBody] ResetPasswordRequest req, CancellationToken ct)
    {
        var r = await _svc.ResetPasswordAsync(adminId, req, ct);
        return r.Success ? Ok(r) : BadRequest(r);
    }

    [HttpPost("departments")]
    public async Task<ActionResult> CreateDepartment([FromBody] UpsertDepartmentRequest req, CancellationToken ct)
    {
        var r = await _svc.CreateDepartmentAsync(req, ct);
        return r.Success ? Ok(r) : BadRequest(r);
    }

    [HttpPut("departments/{depId:int}")]
    public async Task<ActionResult> UpdateDepartment(int depId, [FromBody] UpsertDepartmentRequest req, CancellationToken ct)
    {
        var r = await _svc.UpdateDepartmentAsync(depId, req, ct);
        return r.Success ? Ok(r) : BadRequest(r);
    }

    [HttpDelete("departments/{depId:int}")]
    public async Task<ActionResult> DeleteDepartment(int depId, CancellationToken ct)
    {
        var r = await _svc.DeleteDepartmentAsync(depId, ct);
        return r.Success ? Ok(r) : BadRequest(r);
    }

    [HttpPost("categories")]
    public async Task<ActionResult> CreateCategory([FromBody] UpsertCategoryRequest req, CancellationToken ct)
    {
        var r = await _svc.CreateCategoryAsync(req, ct);
        return r.Success ? Ok(r) : BadRequest(r);
    }

    [HttpPut("categories/{id:int}")]
    public async Task<ActionResult> UpdateCategory(int id, [FromBody] UpsertCategoryRequest req, CancellationToken ct)
    {
        var r = await _svc.UpdateCategoryAsync(id, req, ct);
        return r.Success ? Ok(r) : BadRequest(r);
    }

    [HttpDelete("categories/{id:int}")]
    public async Task<ActionResult> DeleteCategory(int id, CancellationToken ct)
    {
        var r = await _svc.DeleteCategoryAsync(id, ct);
        return r.Success ? Ok(r) : BadRequest(r);
    }

    [HttpPost("approvers")]
    public async Task<ActionResult> CreateApprover([FromBody] UpsertApproverRequest req, CancellationToken ct)
    {
        var r = await _svc.CreateApproverAsync(req, ct);
        return r.Success ? Ok(r) : BadRequest(r);
    }

    [HttpPut("approvers/{userId:guid}")]
    public async Task<ActionResult> UpdateApprover(Guid userId, [FromBody] UpsertApproverRequest req, CancellationToken ct)
    {
        var r = await _svc.UpdateApproverAsync(userId, req, ct);
        return r.Success ? Ok(r) : BadRequest(r);
    }

    [HttpDelete("approvers/{userId:guid}")]
    public async Task<ActionResult> DeleteApprover(Guid userId, CancellationToken ct)
    {
        var r = await _svc.DeleteApproverAsync(userId, ct);
        return r.Success ? Ok(r) : BadRequest(r);
    }
}
