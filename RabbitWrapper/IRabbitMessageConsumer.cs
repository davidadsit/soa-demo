using System;

namespace RabbitWrapper
{
    public interface IRabbitMessageConsumer
    {
        IDisposable Subscribe<T>(IQueueName consumerQueueName, Action<T> consumerAction) where T : class;
        void Purge(IQueueName consumerQueueName);
    }
}