using MediatR;
using Microsoft.AspNetCore.Mvc;
using PortfolioManagement.Application.DTOs;
using PortfolioManagement.Domain.Entities;

namespace PortfolioManagement.Api.Controllers;

[Route("api/projects")]
public sealed class ProjectsController : CrudController<Project, ProjectDto, ProjectRequest>
{
    public ProjectsController(IMediator mediator) : base(mediator)
    {
    }
}
