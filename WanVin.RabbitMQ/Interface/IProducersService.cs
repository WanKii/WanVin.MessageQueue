using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanVin.RabbitMQ.Interface
{
    /// <summary>
    /// 生产者服务接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IProducersService
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="item">消息</param>
        /// <returns></returns>
        public Task SendAsync(string queueName, object msg);

        /// <summary>
        /// 发送多条消息
        /// </summary>
        /// <param name="list">消息列表</param>
        /// <returns></returns>
        public Task SendListAsync(string queueName, List<object> list);
    }
}
