using MediatR;
using PortfolioManagement.Application.Common;
using PortfolioManagement.Application.DTOs;

namespace PortfolioManagement.Application.Features.Dashboard;

public sealed record GetDashboardAnalyticsQuery : IRequest<Result<DashboardAnalyticsDto>>;
