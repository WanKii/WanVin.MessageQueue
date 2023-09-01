using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanVin.RabbitMQ.Domain;

namespace WanVin.RabbitMQ
{
    /// <summary>
    /// RabbitMQ扩展类
    /// </summary>
    public static class RabbitMQExtend
    {
        /// <summary>
        /// 获取RabbitMQ连接对象
        /// </summary>
        /// <param name="rabbitMQConfig">RabbitMQ连接配置对象</param>
        /// <returns></returns>
        public static IConnection? RabbitMQConnection(RabbitMQConfig? rabbitMQConfig)
        {
            if (rabbitMQConfig != null && rabbitMQConfig.IsUse)
            {
                var factory = new ConnectionFactory()
                {
                    UserName = rabbitMQConfig.UserName,
                    Password = rabbitMQConfig.Password,
                    HostName = rabbitMQConfig.Host,
                    Port = rabbitMQConfig.Port,
                    VirtualHost = rabbitMQConfig.VirtualHost
                };
                return factory.CreateConnection();
            }
            return null;
        }

        /// <summary>
        /// 初始化通道
        /// </summary>
        /// <param name="channel">通道对象</param>
        /// <param name="rabbitMQConfig">配置文件类</param>
        /// <param name="queueName">队列名称</param>
        public static void InitChannel(this IModel channel, RabbitMQConfig rabbitMQConfig, string queueName)
        {
            //声明一个交换机
            channel.ExchangeDeclare(
                exchange: rabbitMQConfig.ExchangeName,//交换机名称
                type: ExchangeType.Direct,//交换机类型，直连交换机（将消息直接路由到binding key完全匹配的队列里）
                durable: rabbitMQConfig.Durable,//交换机是否持久化
                autoDelete: false,//交换机是否自动删除
                arguments: null//传递额外的参数
                );

            //声明一个队列
            var queueInfo = channel.QueueDeclare(
                queue: queueName,//队列名称
                durable: rabbitMQConfig.Durable,//是否持久化
                exclusive: false,//是否独占
                autoDelete: false,//是否自动删除
                arguments: null//额外传递的参数
                );

            //绑定队列到交换机
            channel.QueueBind(
                queue: queueName,//队列名称
                exchange: rabbitMQConfig.ExchangeName,//交换机名称
                routingKey: queueName,//路由键
                arguments: null//额外传递的参数
                );
        }
    }
}
