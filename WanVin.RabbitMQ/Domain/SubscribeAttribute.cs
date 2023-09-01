namespace WanVin.RabbitMQ.Domain
{
    /// <summary>
    /// 消息队列特性类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class SubscribeAttribute : Attribute
    {
        /// <summary>
        /// 路由key
        /// </summary>
        public string Name { get; }

        public SubscribeAttribute(string name)
        {
            Name = name;
        }
    }
}
