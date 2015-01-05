namespace RabbitWrapper
{
    public interface IQueueName
    {
        ExchangeName Exchange { get; }
        string Purpose { get; }
        string ConsumerName { get; }
        string FullName { get; }
    }
}