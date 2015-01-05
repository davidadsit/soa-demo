using System;
using System.Text;
using EasyNetQ;
using EasyNetQ.Topology;

namespace RabbitWrapper
{
    public class RabbitMessagePublisher : IRabbitMessagePublisher
    {
        readonly IAdvancedBus bus;
        readonly IMessageSerializer messageSerializer;
        readonly IQueueingSettings queueingSettings;

        public RabbitMessagePublisher(IAdvancedBus bus,
            IMessageSerializer messageSerializer,
            IQueueingSettings queueingSettings)
        {
            this.bus = bus;
            this.messageSerializer = messageSerializer;
            this.queueingSettings = queueingSettings;
        }

        public void Publish<T>(ExchangeName exchangeName, T message) where T : class
        {
            var messageBody = messageSerializer.Serialize(message);

            var body = Encoding.UTF8.GetBytes(messageBody);
            var exchange = new Exchange(exchangeName.ToString());

            bus.ExchangeDeclare(name: exchange.Name, type: EasyNetConstants.FanoutExchangeType);
            var messageProperties = new MessageProperties
                                    {
                                        AppId = queueingSettings.ExecutingApplication,
                                        Timestamp = DateTime.Now.SecondsSinceEpoch(),
                                    };
            bus.Publish(exchange, "", false, false, messageProperties, body);
        }
    }
}