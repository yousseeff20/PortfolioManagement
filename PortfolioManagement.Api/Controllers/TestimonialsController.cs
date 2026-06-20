using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioManagement.Application.Common;
using PortfolioManagement.Application.Common.Cqrs;
using PortfolioManagement.Application.DTOs;
using PortfolioManagement.Application.Features.Testimonials;
using PortfolioManagement.Domain.Constants;
using PortfolioManagement.Domain.Entities;

namespace PortfolioManagement.Api.Controllers;

[Route("api/testimonials")]
public sealed class TestimonialsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public TestimonialsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("approved")]
    public async Task<ActionResult<IReadOnlyList<TestimonialDto>>> GetApproved(CancellationToken cancellationToken) =>
        FromResult(await _mediator.Send(new GetApprovedTestimonialsQuery(), cancellationToken));

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet]
    public async Task<ActionResult<PagedResult<TestimonialDto>>> GetPaged([FromQuery] QueryParameters parameters, CancellationToken cancellationToken) =>
        FromResult(await _mediator.Send(new GetPagedQuery<Testimonial, TestimonialDto>(parameters), cancellationToken));

    [HttpPost]
    public async Task<ActionResult<TestimonialDto>> Submit(TestimonialRequest request, CancellationToken cancellationToken) =>
        FromResult(await _mediator.Send(new SubmitTestimonialCommand(request), cancellationToken));

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPatch("{id:guid}/approve")]
    public async Task<ActionResult<TestimonialDto>> Approve(Guid id, CancellationToken cancellationToken) =>
        FromResult(await _mediator.Send(new ReviewTestimonialCommand(id, true), cancellationToken));

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPatch("{id:guid}/reject")]
    public async Task<ActionResult<TestimonialDto>> Reject(Guid id, CancellationToken cancellationToken) =>
        FromResult(await _mediator.Send(new ReviewTestimonialCommand(id, false), cancellationToken));

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken) =>
        FromResult(await _mediator.Send(new DeleteCommand<Testimonial>(id), cancellationToken));
}
