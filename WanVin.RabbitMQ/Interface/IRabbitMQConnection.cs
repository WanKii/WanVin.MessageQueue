using RabbitMQ.Client;

namespace WanVin.RabbitMQ.Interface
{
    public interface IRabbitMQConnection
    {
        IConnection Connection { get; }
    }
}
