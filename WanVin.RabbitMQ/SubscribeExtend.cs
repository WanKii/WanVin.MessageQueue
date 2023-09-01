using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WanVin.RabbitMQ.Domain;
using WanVin.RabbitMQ.Interface;

namespace WanVin.RabbitMQ
{
    /// <summary>
    /// 订阅扩展
    /// </summary>
    public class SubscribeExtend
    {
        private IConnection _connection;//RabbitMQ连接对象
        private IServiceProvider _serviceProvider;//程序服务提供器
        private RabbitMQConfig _rabbitMQConfig;//RabbitMQ配置文件

        public SubscribeExtend(IServiceProvider serviceProvider, RabbitMQConfig rabbitMQConfig, IConnection connection)
        {
            _connection = connection;
            _serviceProvider = serviceProvider;
            _rabbitMQConfig = rabbitMQConfig;
        }

        /// <summary>
        /// 初始化订阅
        /// </summary>
        /// <returns></returns>
        public async Task Init()
        {
            var executorDescriptorList = new List<ConsumerExecutorDescriptor>();
            using var scoped = _serviceProvider.CreateScope();
            var scopedProvider = scoped.ServiceProvider;

            foreach (var item in _rabbitMQConfig.SubscribeList)
            {
                var consumerServices = scopedProvider.GetService(item) as IConsumersSubscribe;
                var typeInfo = consumerServices?.GetType().GetTypeInfo();
                if (!typeof(IConsumersSubscribe).GetTypeInfo().IsAssignableFrom(typeInfo))
                {
                    continue;
                }
                executorDescriptorList.AddRange(GetSubscribeAttributesDescription(typeInfo));
            }

            //订阅信息
            await Subscribe(executorDescriptorList.Where(m => m.Attribute.GetType().Name == typeof(SubscribeAttribute).Name));
        }

        /// <summary>
        /// 订阅信息
        /// </summary>
        /// <param name="ExecutorDescriptorList"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private async Task Subscribe(IEnumerable<ConsumerExecutorDescriptor> ExecutorDescriptorList)
        {
            foreach (var ConsumerExecutorDescriptor in ExecutorDescriptorList)
            {
                using var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
                var queueName = ConsumerExecutorDescriptor.Attribute.Name;
                var provider = scope.ServiceProvider;
                var obj = ActivatorUtilities.GetServiceOrCreateInstance(provider, ConsumerExecutorDescriptor.ImplTypeInfo);
                ParameterInfo[] parameterInfos = ConsumerExecutorDescriptor.MethodInfo.GetParameters();

                //创建通道
                var channel = _connection.CreateModel();

                //初始化通道
                channel.InitChannel(_rabbitMQConfig, queueName);

                //创建事件消费者
                var consumer = new EventingBasicConsumer(channel);

                //订阅触发回调事件
                consumer.Received += async (model, ea) =>
                {
                    if (ea.Body.ToArray() != null)
                    {
                        //反序列化获取消息
                        string msg = Encoding.UTF8.GetString(ea.Body.ToArray()) ?? throw new ArgumentNullException("RabbitMQ没有消息内容！");

                        //消息确认
                        channel.BasicAck(ea.DeliveryTag, false);

                        //创建子线程异步执行反射业务方法
                        await Task.Run(async () =>
                        {
                            //反射调用消息处理方法
                            if (parameterInfos.Length == 0)
                            {
                                //无参
                                ConsumerExecutorDescriptor.MethodInfo.Invoke(obj, null);
                            }
                            else
                            {
                                //有参
                                object[] parameters = new object[] { msg };
                                ConsumerExecutorDescriptor.MethodInfo.Invoke(obj, parameters);
                            }
                            await Task.CompletedTask;
                        });
                    }
                };

                //消费
                channel.BasicConsume(
                    queue: queueName,//队列名称
                    autoAck: false,//是否自动确认
                    consumer: consumer//消费者
                    );
            }
            await Task.CompletedTask;
        }


        private IEnumerable<ConsumerExecutorDescriptor> GetSubscribeAttributesDescription(TypeInfo typeInfo)
        {
            foreach (var method in typeInfo.DeclaredMethods)
            {
                var topicAttr = method.GetCustomAttributes<SubscribeAttribute>(true);
                var SubscribeAttributes = topicAttr as IList<SubscribeAttribute> ?? topicAttr.ToList();

                if (!SubscribeAttributes.Any())
                {
                    continue;
                }

                foreach (var attr in SubscribeAttributes)
                {
                    yield return InitDescriptor(attr, method, typeInfo);
                }
            }
        }


        private ConsumerExecutorDescriptor InitDescriptor(SubscribeAttribute attr, MethodInfo methodInfo, TypeInfo implType)
        {
            var descriptor = new ConsumerExecutorDescriptor
            {
                Attribute = attr,
                MethodInfo = methodInfo,
                ImplTypeInfo = implType
            };

            return descriptor;
        }

        public class ConsumerExecutorDescriptor
        {
            public MethodInfo MethodInfo { get; set; }

            public TypeInfo ImplTypeInfo { get; set; }

            public SubscribeAttribute Attribute { get; set; }
        }

    }
}
