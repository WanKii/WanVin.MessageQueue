using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using WanVin.RabbitMQ.Core;
using WanVin.RabbitMQ.Domain;
using WanVin.RabbitMQ.Interface;

namespace WanVin.RabbitMQ.Service
{
    public class BaseProducersService : IProducersService
    {
        private readonly IConnection _connection;//RabbitMQ连接对象
        private readonly RabbitMQConfig _rabbitMQConfig;//RabbitMQ配置文件

        public BaseProducersService(
            IConnection connection,
            RabbitMQConfig rabbitMQConfig)
        {
            _connection = connection;
            _rabbitMQConfig = rabbitMQConfig;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public async Task SendAsync(string queueName, object msg)
        {
            if (msg != null)
            {
                //创建通道
                var channel = _connection.CreateModel();

                //初始化通道
                channel.InitChannel(_rabbitMQConfig, queueName);

                //序列化消息
                string msginfo = msg is string ? msg.ToString() : JsonConvert.SerializeObject(msg);

                //转化为流数据
                var body = Encoding.UTF8.GetBytes(msginfo);

                //标记消息特性
                var properties = channel.CreateBasicProperties();

                //是否持久化
                properties.DeliveryMode = _rabbitMQConfig.Durable ? (byte)2 : (byte)1;

                //发布消息
                channel.BasicPublish(
                    exchange: _rabbitMQConfig.ExchangeName,
                    routingKey: queueName,
                    basicProperties: properties,
                    body: body
                    );

                //关闭通道
                channel.Close();

                await Task.CompletedTask;
            }
        }

        /// <summary>
        /// 发送多条消息
        /// </summary>
        /// <param name="list">消息列表</param>
        /// <returns></returns>
        public async Task SendListAsync(string queueName, List<object> list)
        {
            if (list != null && list.Any())
            {
                //创建通道
                var channel = _connection.CreateModel();

                //初始化通道
                channel.InitChannel(_rabbitMQConfig, queueName);

                foreach (var msg in list)
                {
                    //序列化消息
                    string msginfo = msg is string ? msg.ToString() : JsonConvert.SerializeObject(msg);

                    //转化为流数据
                    var body = Encoding.UTF8.GetBytes(msginfo);

                    //标记消息特性
                    var properties = channel.CreateBasicProperties();

                    //是否持久化
                    properties.DeliveryMode = _rabbitMQConfig.Durable ? (byte)2 : (byte)1;

                    //发布消息
                    channel.BasicPublish(
                        exchange: _rabbitMQConfig.ExchangeName,
                        routingKey: queueName,
                        basicProperties: properties,
                        body: body
                        );
                }

                //关闭通道
                channel.Close();

                await Task.CompletedTask;
            }
        }
    }
}
