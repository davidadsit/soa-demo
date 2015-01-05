using System;
using log4net;
using RabbitWrapper;

namespace EmailService
{
    public class RabbitQueueListener
    {
        static ILog logger = LogManager.GetLogger(typeof(RabbitQueueListener));
        public static readonly ExchangeName MailRequestExchangeName = new ExchangeName("mailrequest", 1);
        
        readonly IRabbitMessageConsumer messageConsumer;
        readonly IQueueFactory queueFactory;
        readonly IRabbitMessagePublisher rabbitMessagePublisher;
        IDisposable emailRequestMessageDisposable;

        string pairName = "change-this-to-your-names";

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
            emailRequestMessageDisposable = messageConsumer.Subscribe<EmailRequestMessage>(queueFactory.Create(MailRequestExchangeName, "email", pairName), MandrillHandler);
        }
        
        public void Stop()
        {
            emailRequestMessageDisposable.Dispose();
        }

        public void MandrillHandler(EmailRequestMessage message)
        {

            logger.InfoFormat("Email Message Send!{0}To: {1}{0}From: {2}{0}Subject: {3}{0}Message: {0}{4}{0}{0}", Environment.NewLine, message.To, message.From, message.Subject, message.Message);
        }
    }
}