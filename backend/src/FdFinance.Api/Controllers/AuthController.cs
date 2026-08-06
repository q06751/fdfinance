using System.Security.Claims;
using FdFinance.Application.DTOs;
using FdFinance.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FdFinance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResult<LoginResponse>>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _auth.LoginAsync(request, ct);
        if (!result.Success) return Unauthorized(result);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult> Me(CancellationToken ct)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id)) return Unauthorized();
        var admin = await _auth.GetAdminAsync(id, ct);
        if (admin is null) return NotFound();
        return Ok(ApiResult<object>.Ok(new
        {
            adminId = admin.F_AdminId,
            loginName = admin.F_LoginNmae,
            departmentName = admin.F_DName,
            depId = admin.F_Depid,
            role = admin.ResolveRole()
        }));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest req, CancellationToken ct)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id)) return Unauthorized();
        var result = await _auth.ChangePasswordAsync(id, req, ct);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
