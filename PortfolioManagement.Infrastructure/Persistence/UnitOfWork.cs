using PortfolioManagement.Application.Interfaces;
using PortfolioManagement.Domain.Common;
using PortfolioManagement.Infrastructure.Persistence.Repositories;

namespace PortfolioManagement.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(ApplicationDbContext context) => _context = context;

    public IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        var type = typeof(TEntity);
        if (_repositories.TryGetValue(type, out var repository))
        {
            return (IRepository<TEntity>)repository;
        }

        var created = new GenericRepository<TEntity>(_context);
        _repositories[type] = created;
        return created;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        _repositories.Clear();
        _context.Dispose();
    }
}
