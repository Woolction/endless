using RabbitMQ.Client;

namespace Domain.Common.Interfaces.Db;

public interface IRabbitMqConnector
{
    public IConnection Connection { get; set; }

    Task CreateConnectionAsync();
}