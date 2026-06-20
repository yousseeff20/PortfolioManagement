using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioManagement.Application.Common;
using PortfolioManagement.Application.Common.Cqrs;
using PortfolioManagement.Application.DTOs;
using PortfolioManagement.Application.Features.ContactMessages;
using PortfolioManagement.Domain.Constants;
using PortfolioManagement.Domain.Entities;

namespace PortfolioManagement.Api.Controllers;

[Route("api/contact-messages")]
public sealed class ContactMessagesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public ContactMessagesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<ContactMessageDto>> Submit(ContactMessageRequest request, CancellationToken cancellationToken) =>
        FromResult(await _mediator.Send(new SubmitContactMessageCommand(request), cancellationToken));

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet]
    public async Task<ActionResult<PagedResult<ContactMessageDto>>> GetPaged([FromQuery] QueryParameters parameters, CancellationToken cancellationToken) =>
        FromResult(await _mediator.Send(new GetPagedQuery<ContactMessage, ContactMessageDto>(parameters), cancellationToken));

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPatch("{id:guid}/read")]
    public async Task<ActionResult<ContactMessageDto>> MarkRead(Guid id, CancellationToken cancellationToken) =>
        FromResult(await _mediator.Send(new MarkContactMessageReadCommand(id), cancellationToken));

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken) =>
        FromResult(await _mediator.Send(new DeleteCommand<ContactMessage>(id), cancellationToken));
}
