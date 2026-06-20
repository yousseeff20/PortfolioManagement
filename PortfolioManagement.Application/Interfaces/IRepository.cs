using PortfolioManagement.Domain.Common;

namespace PortfolioManagement.Application.Interfaces;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    IQueryable<TEntity> Query(bool asNoTracking = true);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void SoftDelete(TEntity entity);
    void Delete(TEntity entity);
}
