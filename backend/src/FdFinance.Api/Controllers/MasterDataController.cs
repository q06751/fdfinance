using FdFinance.Application.DTOs;
using FdFinance.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FdFinance.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MasterDataController : ControllerBase
{
    private readonly IMasterDataService _svc;
    private readonly ICacheService _cache;

    public MasterDataController(IMasterDataService svc, ICacheService cache)
    {
        _svc = svc;
        _cache = cache;
    }

    [HttpGet("categories")]
    public async Task<ActionResult> Categories([FromQuery] string? code, CancellationToken ct)
        => Ok(ApiResult<object>.Ok(await _svc.GetCategoriesAsync(code, ct)));

    [HttpGet("users")]
    public async Task<ActionResult> Users(CancellationToken ct)
        => Ok(ApiResult<object>.Ok(await _svc.GetUsersAsync(ct)));

    [HttpGet("departments")]
    public async Task<ActionResult> Departments(CancellationToken ct)
        => Ok(ApiResult<object>.Ok(await _svc.GetDepartmentsAsync(ct)));

    [HttpGet("applies")]
    public async Task<ActionResult> Applies(CancellationToken ct)
        => Ok(ApiResult<object>.Ok(await _svc.GetAppliesAsync(ct)));

    [HttpPost("applies")]
    public async Task<ActionResult> CreateApply([FromBody] CreateApplyRequest req, CancellationToken ct)
    {
        var result = await _svc.CreateApplyAsync(req, ct);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("health")]
    [AllowAnonymous]
    public ActionResult Health()
        => Ok(new
        {
            status = "ok",
            redis = _cache.IsRedisConnected,
            time = DateTime.UtcNow
        });
}
