using FdFinance.Application.DTOs;
using FdFinance.Application.Interfaces;
using FdFinance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FdFinance.Application.Services;

public class MasterDataService : IMasterDataService
{
    private readonly IFinanceDbContext _db;
    private readonly ICacheService _cache;

    public MasterDataService(IFinanceDbContext db, ICacheService cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(string? code = null, CancellationToken ct = default)
    {
        var cacheKey = $"master:cats:{code ?? "all"}";
        var cached = await _cache.GetAsync<List<CategoryDto>>(cacheKey, ct);
        if (cached is not null) return cached;

        var q = _db.Categories.AsNoTracking().Where(c => !c.F_Isdelete);
        if (!string.IsNullOrWhiteSpace(code))
            q = q.Where(c => c.F_Code == code);

        var list = await q.OrderBy(c => c.F_Id)
            .Select(c => new CategoryDto(c.F_Id, c.F_Code, c.F_Name, c.F_CreateTime))
            .ToListAsync(ct);

        await _cache.SetAsync(cacheKey, list, TimeSpan.FromMinutes(10), ct);
        return list;
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken ct = default)
    {
        var cacheKey = "master:users";
        var cached = await _cache.GetAsync<List<UserDto>>(cacheKey, ct);
        if (cached is not null) return cached;

        var list = await _db.Users.AsNoTracking()
            .Where(u => u.F_IsDelete == 0)
            .OrderBy(u => u.F_Name)
            .Select(u => new UserDto(u.F_UserId, u.F_Name, u.F_Phone, u.F_Code))
            .ToListAsync(ct);

        await _cache.SetAsync(cacheKey, list, TimeSpan.FromMinutes(10), ct);
        return list;
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetDepartmentsAsync(CancellationToken ct = default)
    {
        var cacheKey = "master:deps";
        var cached = await _cache.GetAsync<List<DepartmentDto>>(cacheKey, ct);
        if (cached is not null) return cached;

        var list = await _db.Departments.AsNoTracking()
            .Where(d => d.IsDelete == 0)
            .OrderBy(d => d.DepId)
            .Select(d => new DepartmentDto(d.DepId, d.DName, d.ClassCode))
            .ToListAsync(ct);

        await _cache.SetAsync(cacheKey, list, TimeSpan.FromMinutes(10), ct);
        return list;
    }

    public async Task<IReadOnlyList<ApplyDto>> GetAppliesAsync(CancellationToken ct = default)
    {
        return await _db.Applies.AsNoTracking()
            .Where(a => a.F_IsDelete == 0)
            .OrderByDescending(a => a.F_CreateDate)
            .Select(a => new ApplyDto(a.F_ApplyId, a.F_Name, a.F_Department, a.F_Money, a.F_Descripion, a.F_CreateDate))
            .ToListAsync(ct);
    }

    public async Task<ApiResult<Guid>> CreateApplyAsync(CreateApplyRequest req, CancellationToken ct = default)
    {
        var id = Guid.NewGuid();
        _db.Applies.Add(new T_Apply
        {
            F_ApplyId = id,
            F_Name = req.Name,
            F_Department = req.Department,
            F_Money = req.Money,
            F_Descripion = req.Descripion,
            F_CreateDate = DateTime.Now,
            F_IsDelete = 0
        });
        await _db.SaveChangesAsync(ct);
        return ApiResult<Guid>.Ok(id, "申请已创建");
    }
}
