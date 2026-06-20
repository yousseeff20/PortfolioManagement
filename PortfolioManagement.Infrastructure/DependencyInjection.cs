using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortfolioManagement.Application.Interfaces;
using PortfolioManagement.Domain.Entities;
using PortfolioManagement.Infrastructure.Options;
using PortfolioManagement.Infrastructure.Persistence;
using PortfolioManagement.Infrastructure.Services;

namespace PortfolioManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.Configure<AdminSeedOptions>(configuration.GetSection("AdminSeed"));
        services.Configure<LocalStorageOptions>(configuration.GetSection("Storage:Local"));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Use AddIdentityCore instead of AddIdentity to avoid registering
        // cookie-based auth handlers that override the JWT Bearer default scheme.
        // AddIdentity sets DefaultAuthenticateScheme / DefaultChallengeScheme to
        // cookie schemes, causing [Authorize] to return 302 redirects to a
        // nonexistent login page instead of 401 JSON responses.
        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}
