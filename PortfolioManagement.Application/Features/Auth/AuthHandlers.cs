using MediatR;
using PortfolioManagement.Application.Common;
using PortfolioManagement.Application.DTOs.Auth;
using PortfolioManagement.Application.Interfaces;

namespace PortfolioManagement.Application.Features.Auth;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService) => _authService = authService;

    public Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken) =>
        _authService.LoginAsync(request.Request, cancellationToken);
}

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IAuthService authService) => _authService = authService;

    public Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken) =>
        _authService.RefreshAsync(request.Request, cancellationToken);
}

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IAuthService _authService;

    public LogoutCommandHandler(IAuthService authService) => _authService = authService;

    public Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken) =>
        _authService.LogoutAsync(request.Request, cancellationToken);
}
