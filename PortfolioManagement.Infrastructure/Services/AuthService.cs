using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PortfolioManagement.Application.Common;
using PortfolioManagement.Application.DTOs.Auth;
using PortfolioManagement.Application.Interfaces;
using PortfolioManagement.Domain.Constants;
using PortfolioManagement.Domain.Entities;
using PortfolioManagement.Infrastructure.Options;
using PortfolioManagement.Infrastructure.Persistence;

namespace PortfolioManagement.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtOptions _jwtOptions;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _context = context;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Result<AuthResponse>.Failure("InvalidCredentials", "Invalid email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(AppRoles.Admin))
        {
            return Result<AuthResponse>.Failure("Forbidden", "Only admin users can sign in.");
        }

        return await IssueTokensAsync(user, roles, cancellationToken);
    }

    public async Task<Result<AuthResponse>> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(request.RefreshToken);
        var refreshToken = await _context.RefreshTokens.Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (refreshToken?.User is null || !refreshToken.IsActive)
        {
            return Result<AuthResponse>.Failure("InvalidRefreshToken", "Refresh token is invalid or expired.");
        }

        refreshToken.RevokedAt = DateTimeOffset.UtcNow;
        var roles = await _userManager.GetRolesAsync(refreshToken.User);
        var issued = await IssueTokensAsync(refreshToken.User, roles, cancellationToken);

        if (issued.Succeeded && issued.Value is not null)
        {
            refreshToken.ReplacedByTokenHash = HashToken(issued.Value.RefreshToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return issued;
    }

    public async Task<Result> LogoutAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(request.RefreshToken);
        var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
        if (refreshToken is not null)
        {
            refreshToken.RevokedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }

    private async Task<Result<AuthResponse>> IssueTokensAsync(ApplicationUser user, IEnumerable<string> roles, CancellationToken cancellationToken)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        var refreshToken = GenerateRefreshToken();
        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = HashToken(refreshToken),
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)
        });
        await _context.SaveChangesAsync(cancellationToken);

        return Result<AuthResponse>.Success(new AuthResponse(
            new JwtSecurityTokenHandler().WriteToken(jwt),
            refreshToken,
            expiresAt,
            user.Email ?? string.Empty,
            user.FullName,
            roles.ToList()));
    }

    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private static string HashToken(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }
}
