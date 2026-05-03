namespace Nunari.Auth.Infrastructure.Interfaces;

public interface IRabbitMqPublisher
{
    Task PublishAsync<T>(string routingKey, T message);
}
