namespace PortfolioManagement.Application.DTOs.Auth;

public sealed record LoginRequest(string Email, string Password);
public sealed record RefreshTokenRequest(string RefreshToken);

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset ExpiresAt,
    string Email,
    string FullName,
    IReadOnlyList<string> Roles);
