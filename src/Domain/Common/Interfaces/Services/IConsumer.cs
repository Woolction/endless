using RabbitMQ.Client;

namespace Domain.Common.Interfaces.Services;

public interface IConsumer : IDisposable
{
    Task Consume(IConnection connection, CancellationToken token);
}