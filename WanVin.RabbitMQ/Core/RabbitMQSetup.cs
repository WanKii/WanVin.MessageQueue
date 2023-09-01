using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WanVin.RabbitMQ.Interface;
using WanVin.RabbitMQ.Service;
using WanVin.RabbitMQ.Domain;

namespace WanVin.RabbitMQ.Core
{
    public static class RabbitMQSetup
    {
        /// <summary>
        /// 添加RabbitMQ订阅
        /// </summary>
        /// <param name="services">程序服务</param>
        /// <param name="rabbitMQConfig">配置文件类</param>
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