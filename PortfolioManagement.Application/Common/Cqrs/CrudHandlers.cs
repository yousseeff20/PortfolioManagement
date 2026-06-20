using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfolioManagement.Application.Common;
using PortfolioManagement.Application.Interfaces;
using PortfolioManagement.Domain.Common;

namespace PortfolioManagement.Application.Common.Cqrs;

public sealed class GetPagedQueryHandler<TEntity, TDto> : IRequestHandler<GetPagedQuery<TEntity, TDto>, Result<PagedResult<TDto>>>
    where TEntity : BaseEntity
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<TDto>>> Handle(GetPagedQuery<TEntity, TDto> request, CancellationToken cancellationToken)
    {
        var parameters = request.Parameters;
        var pageNumber = Math.Max(parameters.PageNumber, 1);
        var pageSize = Math.Clamp(parameters.PageSize, 1, 100);
        var query = _unitOfWork.Repository<TEntity>().Query();

        // Search: build a dynamic OR filter over all string properties on the entity.
        // entity.ToString() is NOT translatable to SQL and would throw at runtime.
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.Trim().ToLowerInvariant();
            var param = Expression.Parameter(typeof(TEntity), "e");

            var stringProperties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(string) && p.CanRead)
                .ToList();

            Expression? combined = null;
            var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
            var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
            var searchConstant = Expression.Constant(search);

            foreach (var prop in stringProperties)
            {
                // e.PropertyName
                var propertyAccess = Expression.Property(param, prop);
                // e.PropertyName != null
                var notNull = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));
                // e.PropertyName.ToLower()
                var toLower = Expression.Call(propertyAccess, toLowerMethod);
                // e.PropertyName.ToLower().Contains(search)
                var contains = Expression.Call(toLower, containsMethod, searchConstant);
                // e.PropertyName != null && e.PropertyName.ToLower().Contains(search)
                var safe = Expression.AndAlso(notNull, contains);

                combined = combined is null ? safe : Expression.OrElse(combined, safe);
            }

            if (combined is not null)
            {
                var lambda = Expression.Lambda<Func<TEntity, bool>>(combined, param);
                query = query.Where(lambda);
            }
        }

        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        var total = await query.CountAsync(cancellationToken);
        var entities = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        var dtos = _mapper.Map<IReadOnlyList<TDto>>(entities);

        return Result<PagedResult<TDto>>.Success(new PagedResult<TDto>(dtos, pageNumber, pageSize, total));
    }

    private static IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, string? sortBy, bool descending)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return descending ? query.OrderByDescending(e => e.CreatedAt) : query.OrderBy(e => e.CreatedAt);
        }

        var property = typeof(TEntity).GetProperties()
            .FirstOrDefault(p => string.Equals(p.Name, sortBy, StringComparison.OrdinalIgnoreCase));

        if (property is null)
        {
            return query.OrderByDescending(e => e.CreatedAt);
        }

        // Build a properly-typed lambda: x => x.Property (not boxed to object).
        // Expression.Convert to typeof(object) is NOT translatable by EF Core.
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var propertyAccess = Expression.Property(parameter, property);
        var lambda = Expression.Lambda(propertyAccess, parameter);

        var methodName = descending ? "OrderByDescending" : "OrderBy";
        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(TEntity), property.PropertyType);

        return (IQueryable<TEntity>)method.Invoke(null, [query, lambda])!;
    }
}

public sealed class GetByIdQueryHandler<TEntity, TDto> : IRequestHandler<GetByIdQuery<TEntity, TDto>, Result<TDto>>
    where TEntity : BaseEntity
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TDto>> Handle(GetByIdQuery<TEntity, TDto> request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.Repository<TEntity>().GetByIdAsync(request.Id, cancellationToken);
        return entity is null
            ? Result<TDto>.Failure("NotFound", $"{typeof(TEntity).Name} was not found.")
            : Result<TDto>.Success(_mapper.Map<TDto>(entity));
    }
}

public sealed class CreateCommandHandler<TEntity, TRequest, TDto> : IRequestHandler<CreateCommand<TEntity, TRequest, TDto>, Result<TDto>>
    where TEntity : BaseEntity
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TDto>> Handle(CreateCommand<TEntity, TRequest, TDto> request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<TEntity>(request.Request);
        await _unitOfWork.Repository<TEntity>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<TDto>.Success(_mapper.Map<TDto>(entity));
    }
}

public sealed class UpdateCommandHandler<TEntity, TRequest, TDto> : IRequestHandler<UpdateCommand<TEntity, TRequest, TDto>, Result<TDto>>
    where TEntity : BaseEntity
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TDto>> Handle(UpdateCommand<TEntity, TRequest, TDto> request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<TEntity>();
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Result<TDto>.Failure("NotFound", $"{typeof(TEntity).Name} was not found.");
        }

        _mapper.Map(request.Request, entity);
        // Do NOT call repository.Update(entity) — entity is already tracked by GetByIdAsync.
        // Do NOT manually set UpdatedAt — SaveChangesAsync override handles it.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<TDto>.Success(_mapper.Map<TDto>(entity));
    }
}

public sealed class DeleteCommandHandler<TEntity> : IRequestHandler<DeleteCommand<TEntity>, Result>
    where TEntity : BaseEntity
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCommand<TEntity> request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<TEntity>();
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Result.Failure("NotFound", $"{typeof(TEntity).Name} was not found.");
        }

        repository.SoftDelete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
