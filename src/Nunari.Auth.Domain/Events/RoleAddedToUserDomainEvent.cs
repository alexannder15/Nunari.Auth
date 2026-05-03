using Nunari.Auth.Domain.Interfaces;

namespace Nunari.Auth.Domain.Events;

public record RoleAddedToUserDomainEvent(Guid Id, string RoleName) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
