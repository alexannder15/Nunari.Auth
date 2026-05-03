using Nunari.Auth.Domain.AggregateRoot;

namespace Nunari.Auth.Application.Interfaces;

public interface IUserContextService
{
    Task<User>? GetCurrentUserAsync();
}
