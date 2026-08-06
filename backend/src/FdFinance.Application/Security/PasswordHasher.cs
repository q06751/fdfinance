using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace FdFinance.Application.Security;

/// <summary>
/// 密码校验/写入策略，兼容老库 FormsAuthentication MD5（大写 32 位 hex，无前缀）。
/// 写入模式见配置 Password:WriteMode = Bcrypt | LegacyMd5
/// 双跑期若老系统仍要验密，须用 LegacyMd5。
/// </summary>
public static class PasswordHasher
{
    private static readonly Regex Hex32 = new("^[0-9a-fA-F]{32}$", RegexOptions.Compiled);

    public static bool Verify(string input, string? stored)
    {
        if (string.IsNullOrEmpty(stored)) return false;

        // BCrypt
        if (stored.StartsWith("$2", StringComparison.Ordinal))
        {
            try { return BCrypt.Net.BCrypt.Verify(input, stored); }
            catch { return false; }
        }

        // 显式 md5: 前缀（演示/迁移）
        if (stored.StartsWith("md5:", StringComparison.OrdinalIgnoreCase))
            return string.Equals(Md5Hex(input), stored[4..], StringComparison.OrdinalIgnoreCase);

        // 老库：FormsAuthentication.HashPasswordForStoringInConfigFile → 32 位 hex（通常大写）
        if (Hex32.IsMatch(stored))
            return string.Equals(Md5Hex(input), stored, StringComparison.OrdinalIgnoreCase);

        // 极旧明文
        return stored == input;
    }

    /// <summary>按配置写入。默认 Bcrypt；双跑写 LegacyMd5。</summary>
    public static string Hash(string plain, IConfiguration? config = null)
    {
        var mode = (config?["Password:WriteMode"] ?? "Bcrypt").Trim();
        if (mode.Equals("LegacyMd5", StringComparison.OrdinalIgnoreCase)
            || mode.Equals("Md5", StringComparison.OrdinalIgnoreCase))
        {
            // 与 DEncrypt.GetMD5Encript / FormsAuthentication 一致：大写 hex
            return Md5Hex(plain).ToUpperInvariant();
        }

        return BCrypt.Net.BCrypt.HashPassword(plain);
    }

    public static string Md5Hex(string source)
    {
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes(source));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
