using MediatR;
using Microsoft.AspNetCore.Mvc;
using PortfolioManagement.Application.DTOs;
using PortfolioManagement.Domain.Entities;

namespace PortfolioManagement.Api.Controllers;

[Route("api/about-me")]
public sealed class AboutMeController : CrudController<AboutMe, AboutMeDto, AboutMeRequest>
{
    public AboutMeController(IMediator mediator) : base(mediator)
    {
    }
}
