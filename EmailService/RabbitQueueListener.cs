using System;
using log4net;
using Newtonsoft.Json;
using RabbitWrapper;

namespace EmailService
{
    public class RabbitQueueListener
    {
        static ILog logger = LogManager.GetLogger("RabbitQueueListener");
        public static readonly ExchangeName MailRequestExchangeName = new ExchangeName("emailrequested", 1);

        readonly IRabbitMessageConsumer messageConsumer;
        readonly IQueueFactory queueFactory;
        readonly IRabbitMessagePublisher rabbitMessagePublisher;
        IDisposable emailRequestMessageDisposable;

        public RabbitQueueListener(IRabbitMessageConsumer messageConsumer,
            IQueueFactory queueFactory,
            IRabbitMessagePublisher rabbitMessagePublisher)
        {
            this.messageConsumer = messageConsumer;
            this.queueFactory = queueFactory;
            this.rabbitMessagePublisher = rabbitMessagePublisher;
        }

        public void Start()
        {
            logger.Info("Started listening for EmailRequested messages");
            logger.Info("---------------------------------------------");
            logger.Info("");
            emailRequestMessageDisposable = messageConsumer.Subscribe<EmailRequestedMessage>(queueFactory.Create(MailRequestExchangeName, "email", "sender"), MandrillHandler);
        }

        public void Stop()
        {
            emailRequestMessageDisposable.Dispose();
        }

        public void MandrillHandler(EmailRequestedMessage message)
        {
            if (string.IsNullOrEmpty(message.To) ||
                string.IsNullOrEmpty(message.From) ||
                string.IsNullOrEmpty(message.Subject) ||
                string.IsNullOrEmpty(message.Message))
            {
                logger.ErrorFormat("Invalid message with CorrelationId '{1}' requested from: {2}{0}", Environment.NewLine, message.CorrelationId, message.AppId);
                return;
            }

            logger.InfoFormat("Email Message Send!{0}To: {1}{0}From: {2}{0}Subject: {3}{0}Message: {0}{4}{0}",
                Environment.NewLine,
                message.To,
                message.From,
                message.Subject,
                message.Message);
        }
    }
}