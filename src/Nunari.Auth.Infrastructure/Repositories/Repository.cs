using Microsoft.EntityFrameworkCore;
using Nunari.Auth.Domain.Interfaces;
using Nunari.Auth.Domain.Models.Common;
using Nunari.Auth.Infrastructure.Context;

namespace Nunari.Auth.Infrastructure.Repositories;

public class Repository<T>(AppDbContext dbContext) : IRepository<T>
    where T : EntityBase
{
    private readonly DbSet<T> _entity = dbContext.Set<T>();

    public IQueryable<T> Query() => _entity;

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _entity.Where(x => !x.IsDeleted).AsNoTracking().ToListAsync();

    public async Task<T?> GetByIdAsync(Guid id) =>
        await _entity.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

    public async Task AddAsync(T entity)
    {
        await _entity.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _entity.Update(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        entity.IsDeleted = true;
        _entity.Update(entity);
        await dbContext.SaveChangesAsync();
    }
}
