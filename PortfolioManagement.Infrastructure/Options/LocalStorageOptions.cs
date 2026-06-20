namespace PortfolioManagement.Infrastructure.Options;

public sealed class LocalStorageOptions
{
    public string RootPath { get; set; } = "wwwroot/uploads";
    public string PublicBasePath { get; set; } = "/uploads";
}
