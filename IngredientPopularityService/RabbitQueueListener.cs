using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using log4net;
using RabbitWrapper;

namespace IngredientPopularityService
{
    public class RabbitQueueListener
    {
        static ILog logger = LogManager.GetLogger("RabbitQueueListener");
        public static readonly ExchangeName PizzaRequestExchangeName = new ExchangeName("pizzarequested", 1);
        static readonly Dictionary<string, int> toppingPopularity = new Dictionary<string, int>(); 

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
            PrintAnalysis();
            emailRequestMessageDisposable = messageConsumer.Subscribe<PizzaRequestedMessage>(queueFactory.Create(PizzaRequestExchangeName, "marketing", "ingredientanalyst1"), MandrillHandler);
        }

        static void PrintAnalysis()
        {
            Console.Clear();

            logger.Info("Analyzing topping popularity");
            logger.Info("---------------------------------------------");
            logger.Info("");

            foreach (var topping in toppingPopularity.OrderByDescending(x => x.Value))
            {
                logger.InfoFormat("{0}: {1}", topping.Key, topping.Value);
            }
        }

        public void Stop()
        {
            emailRequestMessageDisposable.Dispose();
        }

        public void MandrillHandler(PizzaRequestedMessage message)
        {
            if (message.Toppings.Length <= 0)
            {
                return;                                
            }

            foreach (var topping in message.Toppings)
            {
                if (toppingPopularity.ContainsKey(topping))
                {
                    toppingPopularity[topping]++;
                }
                else
                {
                    toppingPopularity[topping] = 1;
                }
            }
            PrintAnalysis();
            Thread.Sleep(50);
        }
    }
}