using Nunari.Auth.Infrastructure.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Nunari.Auth.Infrastructure.Messaging;

internal class RabbitMqPublisher(IConnection connection) : IRabbitMqPublisher
{
    public async Task PublishAsync<T>(string routingKey, T message)
    {
        var channel = await connection.CreateChannelAsync();

        // 1. Declare the exchange
        await channel.ExchangeDeclareAsync(
            exchange: "rootquest.exchange",
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null
        );

        // 2. Declare the queue (if not already declared)
        await channel.QueueDeclareAsync(
            queue: "auth-queue", // You can make this dynamic if needed
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        // 3. Bind the queue to the exchange with the routing key
        await channel.QueueBindAsync(
            queue: "auth-queue",
            exchange: "rootquest.exchange",
            routingKey: routingKey
        );

        // 4. Serialize the message
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        // 5. Create basic properties (optional)
        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
        };

        // 6. Publish the message
        await channel.BasicPublishAsync(
            exchange: "rootquest.exchange",
            routingKey: routingKey,
            mandatory: true,
            basicProperties: props,
            body: body
        );
    }
}
