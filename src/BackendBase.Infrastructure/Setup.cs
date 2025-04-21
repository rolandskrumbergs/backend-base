using BackendBase.Infrastructure.Data;
using BackendBase.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BackendBase.Infrastructure;
public static class Setup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<SoftDeleteInterceptor>();
        services.AddSingleton<AuditingInterceptor>();
        services.AddSingleton<DapperContext>();

        services.AddDbContext<AppDbContext>((serviceProvider, contextOptions) =>
            contextOptions
                .UseSqlServer(
                    configuration.GetConnectionString("SqlConnection"),
                    options => options.EnableRetryOnFailure())
                .AddInterceptors(
                    serviceProvider.GetRequiredService<SoftDeleteInterceptor>(),
                    serviceProvider.GetRequiredService<AuditingInterceptor>()));

        return services;
    }
}
