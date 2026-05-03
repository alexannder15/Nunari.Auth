using Nunari.Auth.Domain.Interfaces;

namespace Nunari.Auth.Domain.Events;

public record UserRegisteredDomainEvent(Guid UserId, string Email) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
