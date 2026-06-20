using MediatR;
using Microsoft.AspNetCore.Mvc;
using PortfolioManagement.Application.DTOs;
using PortfolioManagement.Domain.Entities;

namespace PortfolioManagement.Api.Controllers;

[Route("api/skills")]
public sealed class SkillsController : CrudController<Skill, SkillDto, SkillRequest>
{
    public SkillsController(IMediator mediator) : base(mediator)
    {
    }
}
