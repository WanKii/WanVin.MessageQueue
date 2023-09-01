using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WanVin.RabbitMQ.Interface;
using WanVin.RabbitMQ.Service;
using WanVin.RabbitMQ.Domain;

namespace WanVin.RabbitMQ
{
    public static class RabbitMQSetup
    {
        /// <summary>
        /// 启动RabbitMQ
        /// 生产：await _producersService.SendAsync(MessageQueueKey.XXX, "测试发送消息");
        /// 消费：方法特性：[Subscribe(MessageQueueKey.XXX)]，方法入参：string msg，类继承：IConsumersSubscribe
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddRabbitMQ(this IServiceCollection services, RabbitMQConfig rabbitMQConfig)
        {
            //有订阅类才执行订阅
            if (rabbitMQConfig.SubscribeList != null)
            {
                //连接RabbitMQ
                IConnection? connection = RabbitMQExtend.RabbitMQConnection(rabbitMQConfig);

                if (connection != null)
                {
                    //单例注入rabbitmq连接对象
                    services.AddSingleton(connection);

                    //注入RabbitMQ配置文件
                    services.AddSingleton(rabbitMQConfig);

                    //单例注入订阅类
                    foreach (var item in rabbitMQConfig.SubscribeList)
                        services.TryAddSingleton(item);

                    //注入生产者接口实现类
                    services.AddSingleton<IProducersService, BaseProducersService>();

                    //设置应用程序启动时，HostedService托管服务将自动启动并在后台运行
                    services.AddHostedService<HostedService>();
                }
            }
        }
    }
}