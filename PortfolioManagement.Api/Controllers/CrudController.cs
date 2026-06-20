using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioManagement.Application.Common;
using PortfolioManagement.Application.Common.Cqrs;
using PortfolioManagement.Domain.Common;
using PortfolioManagement.Domain.Constants;

namespace PortfolioManagement.Api.Controllers;

public abstract class CrudController<TEntity, TDto, TRequest> : ApiControllerBase
    where TEntity : BaseEntity
{
    private readonly IMediator _mediator;

    protected CrudController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<PagedResult<TDto>>> GetPaged([FromQuery] QueryParameters parameters, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPagedQuery<TEntity, TDto>(parameters), cancellationToken);
        return FromResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetByIdQuery<TEntity, TDto>(id), cancellationToken);
        return FromResult(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<ActionResult<TDto>> Create([FromBody] TRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateCommand<TEntity, TRequest, TDto>(request), cancellationToken);
        return FromResult(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TDto>> Update(Guid id, [FromBody] TRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateCommand<TEntity, TRequest, TDto>(id, request), cancellationToken);
        return FromResult(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteCommand<TEntity>(id), cancellationToken);
        return FromResult(result);
    }
}
