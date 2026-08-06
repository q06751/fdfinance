using FdFinance.Application.Interfaces;
using FdFinance.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FdFinance.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IReimbursementService, ReimbursementService>();
        services.AddScoped<IMasterDataService, MasterDataService>();
        services.AddScoped<IAdminService, AdminService>();
        return services;
    }
}
