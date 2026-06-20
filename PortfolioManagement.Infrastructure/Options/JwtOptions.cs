namespace PortfolioManagement.Infrastructure.Options;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "PortfolioManagement";
    public string Audience { get; set; } = "PortfolioManagement";
    public string Secret { get; set; } = "CHANGE_ME_TO_A_64_CHARACTER_SECRET_FOR_PRODUCTION_ENVIRONMENTS";
    public int AccessTokenMinutes { get; set; } = 30;
    public int RefreshTokenDays { get; set; } = 14;
}
