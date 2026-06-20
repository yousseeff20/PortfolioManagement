using MediatR;
using Microsoft.AspNetCore.Mvc;
using PortfolioManagement.Application.DTOs;
using PortfolioManagement.Domain.Entities;

namespace PortfolioManagement.Api.Controllers;

[Route("api/experiences")]
public sealed class ExperiencesController : CrudController<Experience, ExperienceDto, ExperienceRequest>
{
    public ExperiencesController(IMediator mediator) : base(mediator)
    {
    }
}
