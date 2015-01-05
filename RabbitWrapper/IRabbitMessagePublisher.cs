namespace RabbitWrapper
{
    public interface IRabbitMessagePublisher
    {
        void Publish<T>(ExchangeName exchangeName, T message) where T : class;
    }
}