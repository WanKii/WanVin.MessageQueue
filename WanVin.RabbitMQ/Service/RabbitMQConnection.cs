using RabbitMQ.Client;
using WanVin.RabbitMQ.Interface;

namespace WanVin.RabbitMQ.Service
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        public IConnection Connection { get; set; }

        public RabbitMQConnection(IConnection connection)
        {
            Connection = connection;
        }
    }
}
