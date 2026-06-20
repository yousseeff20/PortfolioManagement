using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PortfolioManagement.Domain.Constants;
using PortfolioManagement.Domain.Entities;
using PortfolioManagement.Infrastructure.Options;

namespace PortfolioManagement.Infrastructure.Persistence;

public sealed class DatabaseSeeder
{
    private readonly AdminSeedOptions _adminOptions;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public DatabaseSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IOptions<AdminSeedOptions> adminOptions)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _adminOptions = adminOptions.Value;
    }

    public async Task SeedAsync()
    {
        if (!await _roleManager.RoleExistsAsync(AppRoles.Admin))
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(AppRoles.Admin));
            if (!roleResult.Succeeded)
            {
                var message = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Could not create Admin role: {message}");
            }
        }

        var user = await _userManager.FindByEmailAsync(_adminOptions.Email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                Email = _adminOptions.Email,
                UserName = _adminOptions.Email,
                EmailConfirmed = true,
                FullName = _adminOptions.FullName
            };
            var result = await _userManager.CreateAsync(user, _adminOptions.Password);
            if (!result.Succeeded)
            {
                var message = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Could not seed admin user: {message}");
            }
        }

        if (!await _userManager.IsInRoleAsync(user, AppRoles.Admin))
        {
            var roleResult = await _userManager.AddToRoleAsync(user, AppRoles.Admin);
            if (!roleResult.Succeeded)
            {
                var message = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Could not assign Admin role: {message}");
            }
        }
    }
}
