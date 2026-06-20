using MediatR;
using Microsoft.AspNetCore.Mvc;
using PortfolioManagement.Application.DTOs;
using PortfolioManagement.Domain.Entities;

namespace PortfolioManagement.Api.Controllers;

[Route("api/certificates")]
public sealed class CertificatesController : CrudController<Certificate, CertificateDto, CertificateRequest>
{
    public CertificatesController(IMediator mediator) : base(mediator)
    {
    }
}
