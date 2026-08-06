using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FdFinance.Application.DTOs;
using FdFinance.Application.Interfaces;
using FdFinance.Application.Security;
using FdFinance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FdFinance.Application.Services;

public class AuthService : IAuthService
{
    private readonly IFinanceDbContext _db;
    private readonly IConfiguration _config;
    private readonly ICacheService _cache;

    public AuthService(IFinanceDbContext db, IConfiguration config, ICacheService cache)
    {
        _db = db;
        _config = config;
        _cache = cache;
    }

    public async Task<ApiResult<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.LoginName) || string.IsNullOrWhiteSpace(request.Password))
            return ApiResult<LoginResponse>.Fail("请输入账号和密码");

        var name = request.LoginName.Trim();
        var admin = await _db.Admins.AsNoTracking().FirstOrDefaultAsync(a => a.F_LoginNmae == name, ct);
        if (admin is null)
            return ApiResult<LoginResponse>.Fail("账号或者密码错误");

        if (!PasswordHasher.Verify(request.Password, admin.F_PassWord))
            return ApiResult<LoginResponse>.Fail("账号或者密码错误");

        var role = admin.ResolveRole();
        var token = CreateToken(admin, role);
        var resp = new LoginResponse(
            token,
            admin.F_AdminId,
            admin.F_LoginNmae ?? name,
            admin.F_DName ?? "",
            admin.F_Depid,
            role);

        await _cache.SetAsync($"auth:user:{admin.F_AdminId}", StripSecrets(admin), TimeSpan.FromHours(8), ct);
        return ApiResult<LoginResponse>.Ok(resp, "登录成功");
    }

    public async Task<T_Admin?> GetAdminAsync(string adminId, CancellationToken ct = default)
    {
        var cached = await _cache.GetAsync<T_Admin>($"auth:user:{adminId}", ct);
        if (cached is not null)
        {
            cached.F_PassWord = null;
            return cached;
        }

        var admin = await _db.Admins.AsNoTracking().FirstOrDefaultAsync(a => a.F_AdminId == adminId, ct);
        if (admin is null) return null;

        var safe = StripSecrets(admin);
        await _cache.SetAsync($"auth:user:{adminId}", safe, TimeSpan.FromHours(8), ct);
        return safe;
    }

    public async Task<ApiResult<bool>> ChangePasswordAsync(string adminId, ChangePasswordRequest req, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(req.NewPassword) || req.NewPassword.Length < 5)
            return ApiResult<bool>.Fail("新密码至少 5 位");

        var a = await _db.Admins.FirstOrDefaultAsync(x => x.F_AdminId == adminId, ct);
        if (a is null) return ApiResult<bool>.Fail("账号不存在");

        if (!PasswordHasher.Verify(req.OldPassword, a.F_PassWord))
            return ApiResult<bool>.Fail("原密码不正确");

        a.F_PassWord = PasswordHasher.Hash(req.NewPassword, _config);
        await _db.SaveChangesAsync(ct);
        await _cache.RemoveAsync($"auth:user:{adminId}", ct);
        return ApiResult<bool>.Ok(true, "密码已修改");
    }

    private static T_Admin StripSecrets(T_Admin admin) => new()
    {
        F_AdminId = admin.F_AdminId,
        F_LoginNmae = admin.F_LoginNmae,
        F_PassWord = null,
        F_IsUP = admin.F_IsUP,
        F_Depid = admin.F_Depid,
        F_DName = admin.F_DName,
        F_Role = admin.F_Role ?? admin.ResolveRole(),
        ExtensionJson = admin.ExtensionJson
    };

    private string CreateToken(T_Admin admin, string role)
    {
        var key = _config["Jwt:Key"] ?? "FdFinance-Dev-Secret-Key-At-Least-32-Chars!!";
        var issuer = _config["Jwt:Issuer"] ?? "FdFinance";
        var audience = _config["Jwt:Audience"] ?? "FdFinance.Client";
        var hours = int.TryParse(_config["Jwt:ExpireHours"], out var h) ? h : 12;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, admin.F_AdminId),
            new(ClaimTypes.Name, admin.F_LoginNmae ?? ""),
            new("depid", admin.F_Depid.ToString()),
            new("dname", admin.F_DName ?? ""),
            new(ClaimTypes.Role, role)
        };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(hours),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
