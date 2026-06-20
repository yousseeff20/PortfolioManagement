using MediatR;
using PortfolioManagement.Application.Common;
using PortfolioManagement.Domain.Common;

namespace PortfolioManagement.Application.Common.Cqrs;

public sealed record GetPagedQuery<TEntity, TDto>(QueryParameters Parameters) : IRequest<Result<PagedResult<TDto>>>
    where TEntity : BaseEntity;

public sealed record GetByIdQuery<TEntity, TDto>(Guid Id) : IRequest<Result<TDto>>
    where TEntity : BaseEntity;

public sealed record CreateCommand<TEntity, TRequest, TDto>(TRequest Request) : IRequest<Result<TDto>>
    where TEntity : BaseEntity;

public sealed record UpdateCommand<TEntity, TRequest, TDto>(Guid Id, TRequest Request) : IRequest<Result<TDto>>
    where TEntity : BaseEntity;

public sealed record DeleteCommand<TEntity>(Guid Id) : IRequest<Result>
    where TEntity : BaseEntity;
