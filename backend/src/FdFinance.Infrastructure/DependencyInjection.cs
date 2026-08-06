using FdFinance.Application.Interfaces;
using FdFinance.Application.Services;
using FdFinance.Infrastructure.Caching;
using FdFinance.Infrastructure.Data;
using FdFinance.Infrastructure.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace FdFinance.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var conn = config.GetConnectionString("FinanceDb")
                   ?? "Data Source=/workspace/data/fdfinance.db";
        var provider = (config["Database:Provider"] ?? DetectProvider(conn)).Trim();

        services.AddDbContext<FinanceDbContext>(opt =>
        {
            if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)
                || conn.Contains("Server=", StringComparison.OrdinalIgnoreCase)
                || conn.Contains("Initial Catalog=", StringComparison.OrdinalIgnoreCase))
            {
                // 接老 SQL Server：不改表结构；EnsureCreated 关闭
                opt.UseSqlServer(conn);
            }
            else
            {
                opt.UseSqlite(conn);
            }
        });
        services.AddScoped<IFinanceDbContext>(sp => sp.GetRequiredService<FinanceDbContext>());

        services.AddMemoryCache();
        services.AddSingleton<INotificationService, DingTalkNotificationService>();

        var redisConn = config.GetConnectionString("Redis")
                        ?? config["Redis:Configuration"]
                        ?? "localhost:6379";
        var redisEnabled = !string.Equals(config["Redis:Enabled"], "false", StringComparison.OrdinalIgnoreCase);

        IConnectionMultiplexer? mux = null;
        if (redisEnabled)
        {
            try
            {
                var options = ConfigurationOptions.Parse(redisConn);
                options.AbortOnConnectFail = false;
                options.ConnectTimeout = 1500;
                options.SyncTimeout = 1500;
                options.ConnectRetry = 1;
                mux = ConnectionMultiplexer.Connect(options);
                services.AddSingleton<IConnectionMultiplexer>(mux);
                services.AddStackExchangeRedisCache(o =>
                {
                    o.ConfigurationOptions = options;
                    o.InstanceName = "fdfinance:";
                });
            }
            catch (Exception)
            {
                mux = null;
            }
        }

        services.AddSingleton<ICacheService>(sp =>
        {
            var memory = sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
            var logger = sp.GetRequiredService<ILogger<HybridCacheService>>();
            var dist = sp.GetService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            var m = sp.GetService<IConnectionMultiplexer>();
            return new HybridCacheService(memory, logger, dist, m);
        });

        return services;
    }

    private static string DetectProvider(string conn)
        => conn.Contains("Data Source=", StringComparison.OrdinalIgnoreCase)
           && !conn.Contains("Server=", StringComparison.OrdinalIgnoreCase)
            ? "Sqlite"
            : "SqlServer";
}
