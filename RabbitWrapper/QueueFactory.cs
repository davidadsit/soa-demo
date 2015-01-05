using EasyNetQ;

namespace RabbitWrapper
{
    public interface IQueueFactory
    {
        IQueueName Create(ExchangeName exchangeName, string context, string consumerName);
    }

    public class QueueFactory : IQueueFactory
    {
        readonly IAdvancedBus bus;

        public QueueFactory(IAdvancedBus bus)
        {
            this.bus = bus;
        }

        public IQueueName Create(ExchangeName exchangeName, string context, string consumerName)
        {
            var queueName = new QueueName(exchangeName, context, consumerName);

            var exchange = bus.ExchangeDeclare(name: exchangeName.ToString(), type: EasyNetConstants.FanoutExchangeType, durable: true, autoDelete: false, passive: false);

            var queue = bus.QueueDeclare(name: queueName.FullName, durable: true, exclusive: false, autoDelete: false);
            bus.Bind(exchange, queue, "");

            return queueName;
        }

        class QueueName : IQueueName
        {
            public ExchangeName Exchange { get; private set; }
            public string Purpose { get; private set; }
            public string ConsumerName { get; private set; }
            public string FullName { get; private set; }

            public QueueName(ExchangeName exchange, string purpose, string consumerName)
            {
                Exchange = exchange;
                Purpose = purpose;
                ConsumerName = consumerName;
                FullName = string.Format("{0}.{1}.{2}", Exchange, Purpose, ConsumerName);
            }
        }
    }
}