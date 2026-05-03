namespace Nunari.Auth.Domain.Interfaces;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
