using System.Collections.Concurrent;
using System.Text.Json;
using FdFinance.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace FdFinance.Infrastructure.Caching;

/// <summary>
/// Redis-first cache with in-memory fallback when Redis is unavailable.
/// </summary>
public class HybridCacheService : ICacheService
{
    private readonly IMemoryCache _memory;
    private readonly IDistributedCache? _distributed;
    private readonly IConnectionMultiplexer? _mux;
    private readonly ILogger<HybridCacheService> _logger;
    private readonly ConcurrentDictionary<string, byte> _keys = new();
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public HybridCacheService(
        IMemoryCache memory,
        ILogger<HybridCacheService> logger,
        IDistributedCache? distributed = null,
        IConnectionMultiplexer? mux = null)
    {
        _memory = memory;
        _logger = logger;
        _distributed = distributed;
        _mux = mux;
    }

    public bool IsRedisConnected => _mux?.IsConnected == true;

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        if (IsRedisConnected && _distributed is not null)
        {
            try
            {
                var bytes = await _distributed.GetAsync(key, ct);
                if (bytes is { Length: > 0 })
                    return JsonSerializer.Deserialize<T>(bytes, JsonOpts);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis GET failed for {Key}, falling back to memory", key);
            }
        }

        return _memory.TryGetValue(key, out T? val) ? val : default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default)
    {
        var expiry = ttl ?? TimeSpan.FromMinutes(5);
        _keys[key] = 0;
        _memory.Set(key, value, expiry);

        if (IsRedisConnected && _distributed is not null)
        {
            try
            {
                var bytes = JsonSerializer.SerializeToUtf8Bytes(value, JsonOpts);
                await _distributed.SetAsync(key, bytes, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiry
                }, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis SET failed for {Key}", key);
            }
        }
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _memory.Remove(key);
        _keys.TryRemove(key, out _);
        if (IsRedisConnected && _distributed is not null)
        {
            try { await _distributed.RemoveAsync(key, ct); }
            catch (Exception ex) { _logger.LogWarning(ex, "Redis REMOVE failed for {Key}", key); }
        }
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        var matches = _keys.Keys.Where(k => k.StartsWith(prefix, StringComparison.Ordinal)).ToList();
        foreach (var k in matches)
            await RemoveAsync(k, ct);

        if (IsRedisConnected && _mux is not null)
        {
            try
            {
                var server = _mux.GetServers().FirstOrDefault();
                if (server is not null)
                {
                    var db = _mux.GetDatabase();
                    await foreach (var key in server.KeysAsync(pattern: prefix + "*"))
                        await db.KeyDeleteAsync(key);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis prefix remove failed for {Prefix}", prefix);
            }
        }
    }
}
