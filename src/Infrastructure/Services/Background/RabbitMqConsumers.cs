using Microsoft.Extensions.Hosting;
using Domain.Common.Interfaces.Services;
using Infrastructure.Connector;
using Domain.Common.Interfaces.Db;
using Infrastructure.Services.RabbitConsumers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.Background;

public class RabbitMqConsumers : BackgroundService
{
    private readonly IRabbitMqConnector connector;
    private readonly IConsumer[] consumers;

    public RabbitMqConsumers(IRabbitMqConnector connector, IEnumerable<IConsumer> consumers)
    {
        this.connector = connector;
        this.consumers = [.. consumers];
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await connector.CreateConnectionAsync();

        for (int i = 0; i < consumers.Length; i++)
        {
            await consumers[i].Consume(connector.Connection, stoppingToken);
        }

        await Task.Delay(Timeout.Infinite, stoppingToken);

        for (int i = 0; i < consumers.Length; i++)
        {
            consumers[i].Dispose();
        }
    }
}