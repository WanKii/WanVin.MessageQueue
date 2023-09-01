namespace WanVin.RabbitMQ.Domain
{
    /// <summary>
    /// RabbitMQ连接配置对象
    /// </summary>
    public class RabbitMQConfig
    {
        /// <summary>
        /// 是否使用RabbitMQ
        /// </summary>
        public bool IsUse { get; set; } = true;

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 连接地址
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 交换机名称
        /// </summary>
        public string ExchangeName { get; set; } = string.Empty;

        /// <summary>
        /// 是否启用持久化
        /// </summary>
        public bool Durable { get; set; }

        /// <summary>
        /// 虚拟主机（用于逻辑隔离队列）
        /// </summary>
        public string VirtualHost { get; set; } = string.Empty;

        /// <summary>
        /// 需要注入的类集合
        /// </summary>
        public IList<Type> SubscribeList { get; set; } = new List<Type>();
    }
}
