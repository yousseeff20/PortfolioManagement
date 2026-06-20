namespace PortfolioManagement.Infrastructure.Options;

public sealed class AdminSeedOptions
{
    public string Email { get; set; } = "admin@portfolio.local";
    public string Password { get; set; } = "Admin123!ChangeMe";
    public string FullName { get; set; } = "Portfolio Admin";
}
