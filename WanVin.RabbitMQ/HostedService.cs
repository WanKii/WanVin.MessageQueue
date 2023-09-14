using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using WanVin.RabbitMQ.Core;
using WanVin.RabbitMQ.Domain;
using WanVin.RabbitMQ.Interface;

namespace WanVin.RabbitMQ
{
    /// <summary>
    /// RabbitMQ服务启动
    /// </summary>
    public class HostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQConfig _rabbitMQConfig;
        private readonly IRabbitMQConnection _rabbitMQConnection;

        public HostedService(IServiceProvider serviceProvider, RabbitMQConfig rabbitMQConfig, IRabbitMQConnection rabbitMQConnection)
        {
            _serviceProvider = serviceProvider;
            _rabbitMQConfig = rabbitMQConfig;
            _rabbitMQConnection = rabbitMQConnection;
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //启动订阅
            await new SubscribeExtend(_serviceProvider, _rabbitMQConfig, _rabbitMQConnection).Init();
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }
    }
}
