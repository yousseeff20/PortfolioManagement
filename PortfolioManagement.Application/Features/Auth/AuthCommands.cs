using MediatR;
using PortfolioManagement.Application.Common;
using PortfolioManagement.Application.DTOs.Auth;

namespace PortfolioManagement.Application.Features.Auth;

public sealed record LoginCommand(LoginRequest Request) : IRequest<Result<AuthResponse>>;
public sealed record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<Result<AuthResponse>>;
public sealed record LogoutCommand(RefreshTokenRequest Request) : IRequest<Result>;
