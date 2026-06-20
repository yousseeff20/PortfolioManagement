using Microsoft.EntityFrameworkCore;
using PortfolioManagement.Application.Interfaces;
using PortfolioManagement.Domain.Common;

namespace PortfolioManagement.Infrastructure.Persistence.Repositories;

public sealed class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public IQueryable<TEntity> Query(bool asNoTracking = true)
    {
        var query = _dbSet.AsQueryable();
        return asNoTracking ? query.AsNoTracking() : query;
    }

    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        _dbSet.AddAsync(entity, cancellationToken).AsTask();

    public void Update(TEntity entity) => _dbSet.Update(entity);

    public void SoftDelete(TEntity entity)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;
        _dbSet.Update(entity);
    }

    public void Delete(TEntity entity) => _dbSet.Remove(entity);
}
