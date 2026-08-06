using FdFinance.Application.Security;
using Microsoft.Extensions.Configuration;

namespace FdFinance.Tests.Unit;

/// <summary>
/// 密码兼容契约：故意用真实断言，错误实现会立刻红。
/// 覆盖 BCrypt / 无前缀 MD5 / md5: 前缀 / 明文 / 写入模式。
/// </summary>
public class PasswordHasherTests
{
    [Fact]
    public void Verify_Bcrypt_ok()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("admin123");
        Assert.True(PasswordHasher.Verify("admin123", hash));
        Assert.False(PasswordHasher.Verify("wrong", hash));
    }

    [Fact]
    public void Verify_LegacyUpperMd5_no_prefix_ok()
    {
        // FormsAuthentication.HashPasswordForStoringInConfigFile 典型大写 hex
        var stored = PasswordHasher.Md5Hex("legacy123").ToUpperInvariant();
        Assert.Equal(32, stored.Length);
        Assert.True(PasswordHasher.Verify("legacy123", stored));
        Assert.False(PasswordHasher.Verify("legacy124", stored));
    }

    [Fact]
    public void Verify_Md5_prefix_ok()
    {
        var hex = PasswordHasher.Md5Hex("abc");
        Assert.True(PasswordHasher.Verify("abc", "md5:" + hex));
        Assert.False(PasswordHasher.Verify("abd", "md5:" + hex));
    }

    [Fact]
    public void Verify_Plaintext_compat()
    {
        Assert.True(PasswordHasher.Verify("12345", "12345"));
        Assert.False(PasswordHasher.Verify("12345", "54321"));
    }

    [Fact]
    public void Verify_Empty_or_null_fails()
    {
        Assert.False(PasswordHasher.Verify("x", null));
        Assert.False(PasswordHasher.Verify("x", ""));
    }

    [Fact]
    public void Hash_WriteMode_LegacyMd5_is_upper_hex()
    {
        var cfg = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Password:WriteMode"] = "LegacyMd5"
            })
            .Build();
        var h = PasswordHasher.Hash("hello", cfg);
        Assert.Equal(32, h.Length);
        Assert.Equal(h, h.ToUpperInvariant());
        Assert.True(PasswordHasher.Verify("hello", h));
    }

    [Fact]
    public void Hash_WriteMode_Bcrypt_is_bcrypt()
    {
        var cfg = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Password:WriteMode"] = "Bcrypt"
            })
            .Build();
        var h = PasswordHasher.Hash("hello", cfg);
        Assert.StartsWith("$2", h);
        Assert.True(PasswordHasher.Verify("hello", h));
    }

    /// <summary>
    /// 防幻觉：MD5 结果必须与已知向量一致（hello → 5d41402abc4b2a76b9719d911017c592）。
    /// </summary>
    [Fact]
    public void Md5Hex_matches_known_vector()
    {
        Assert.Equal("5d41402abc4b2a76b9719d911017c592", PasswordHasher.Md5Hex("hello"));
    }
}
