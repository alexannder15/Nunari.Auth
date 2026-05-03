using Nunari.Auth.Domain.Interfaces;

namespace Nunari.Auth.Domain.Events;

public record RoleRemovedFromUserDomainEvent(Guid Id, string RoleName) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
