using MediatR;
using Microsoft.AspNetCore.Mvc;
using PortfolioManagement.Application.DTOs;
using PortfolioManagement.Domain.Entities;

namespace PortfolioManagement.Api.Controllers;

[Route("api/educations")]
public sealed class EducationsController : CrudController<Education, EducationDto, EducationRequest>
{
    public EducationsController(IMediator mediator) : base(mediator)
    {
    }
}
