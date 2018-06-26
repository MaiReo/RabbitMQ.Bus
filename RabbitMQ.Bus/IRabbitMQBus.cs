using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Bus
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRabbitMQBusService
    {
        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        void Subscribe<TMessage>() where TMessage : class;

        /// <summary>
        /// 自动订阅消息
        /// </summary>
        void AutoSubscribe();

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="exchangeName">留空则使用默认的交换机</param>
        /// <param name="queueName">队列名</param>
        /// <param name="value">需要发送的消息</param>
        /// <param name="routingKey">路由Key</param>
        void Publish<TMessage>(string exchangeName, string queueName, TMessage value, string routingKey);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="TMessage"><see cref="QueueAttribute"/>标识的类</typeparam>
        /// <param name="value">需要发送的消息</param>
        void Publish<TMessage>(TMessage value);
    }
}
