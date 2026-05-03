using Nunari.Auth.Domain.Models.Common;

namespace Nunari.Auth.Domain.Interfaces;

public interface IRepository<T> where T : EntityBase
{
    IQueryable<T> Query();
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
