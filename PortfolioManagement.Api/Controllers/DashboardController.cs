using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioManagement.Application.DTOs;
using PortfolioManagement.Application.Features.Dashboard;
using PortfolioManagement.Domain.Constants;

namespace PortfolioManagement.Api.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/dashboard")]
public sealed class DashboardController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator) => _mediator = mediator;

    [HttpGet("analytics")]
    public async Task<ActionResult<DashboardAnalyticsDto>> GetAnalytics(CancellationToken cancellationToken) =>
        FromResult(await _mediator.Send(new GetDashboardAnalyticsQuery(), cancellationToken));
}
