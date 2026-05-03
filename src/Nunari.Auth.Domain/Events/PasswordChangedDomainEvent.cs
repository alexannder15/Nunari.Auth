using Nunari.Auth.Domain.Interfaces;

namespace Nunari.Auth.Domain.Events;

public record PasswordChangedDomainEvent(Guid Id) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
