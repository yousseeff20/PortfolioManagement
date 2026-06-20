using PortfolioManagement.Application.Common;
using PortfolioManagement.Application.DTOs.Auth;

namespace PortfolioManagement.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
}
