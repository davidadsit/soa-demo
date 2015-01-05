using System;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Consumer;
using EasyNetQ.Topology;

namespace RabbitWrapper
{
    public class RabbitMessageConsumer : IRabbitMessageConsumer
    {
        private readonly IAdvancedBus bus;
        readonly IMessageSerializer messageSerializer;
        readonly IHandlerCollectionFactory handlerCollectionFactory;

        public RabbitMessageConsumer(IAdvancedBus bus, IMessageSerializer messageSerializer, IHandlerCollectionFactory handlerCollectionFactory)
        {
            this.bus = bus;
            this.messageSerializer = messageSerializer;
            this.handlerCollectionFactory = handlerCollectionFactory;
        }

        public IDisposable Subscribe<T>(IQueueName consumerQueueName, Action<T> consumerAction) where T : class
        {
            var queue = new Queue(consumerQueueName.FullName, true);
            Action<IMessage<T>, MessageReceivedInfo> onMessage = (message, info) => consumerAction(message.Body);
            var handler = (Func<IMessage<T>, MessageReceivedInfo, Task>)((message, info) => TaskHelpers.ExecuteSynchronously(() => onMessage(message, info)));
            return Consume<T>(queue, x1 => x1.Add(handler), x => { });
        }

        private IDisposable Consume<T>(IQueue queue, Action<IHandlerRegistration> addHandlers, Action<IConsumerConfiguration> configure)
        {
            var handlerCollection = handlerCollectionFactory.CreateHandlerCollection();
            addHandlers(handlerCollection);

            return bus.Consume(queue, (body, properties, messageReceivedInfo) =>
            {
                var messageType = typeof(T);
                var handler = handlerCollection.GetHandler(messageType);

                var messageBody = messageSerializer.Deserialize<T>(Encoding.UTF8.GetString(body));
                var message = Message.CreateInstance(messageType, messageBody);
                message.SetProperties(properties);
                return handler(message, messageReceivedInfo);
            }, configure);
        }

        public void Purge(IQueueName consumerQueueName)
        {
            var queue = new Queue(consumerQueueName.FullName, true);
            bus.QueuePurge(queue);
        }
    }
}