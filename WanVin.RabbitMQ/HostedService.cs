﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using WanVin.RabbitMQ.Domain;

namespace WanVin.RabbitMQ
{
    /// <summary>
    /// RabbitMQ服务启动
    /// </summary>
    public class HostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQConfig _rabbitMQConfig;
        private readonly IConnection _connection;

        public HostedService(ILogger<HostedService> logger, IServiceProvider serviceProvider, RabbitMQConfig rabbitMQConfig, IConnection connection)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _rabbitMQConfig = rabbitMQConfig;
            _connection = connection;
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //启动订阅
            await new SubscribeExtend(_serviceProvider, _rabbitMQConfig, _connection).Init();
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