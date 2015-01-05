namespace RabbitWrapper
{
    public interface IQueueingSettings
    {
        string RabbitHosts { get; }
        string RabbitUserName { get; }
        string RabbitPassword { get; }
        string ExecutingApplication { get; }
    }
}