using System;
using System.Threading.Tasks;
using EasyNetQ.Consumer;
using log4net;
using log4net.Config;
using RabbitWrapper;

namespace IngredientPopularityService
{
    class Program
    {
        static ILog logger;

        static void Main()
        {
            try
            {
                XmlConfigurator.Configure();
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                logger = LogManager.GetLogger(typeof (Program));

                var rabbitQueueListener = Initialize();

                var messageListener = Task.Run(() => rabbitQueueListener.Start());
//                Task.WaitAll(messageListener, Task.Delay(TimeSpan.FromMinutes(1)));

//                rabbitQueueListener.Stop();
            }
            catch (Exception x)
            {
                Console.WriteLine(x);
            }
        }

        static RabbitQueueListener Initialize()
        {
            var settings = new Settings();
            var advancedBus = new EasyNetBusFactory(settings).CreateAdvancedBus();
            var rabbitMessageConsumer = new RabbitMessageConsumer(advancedBus, new MessageSerializer(), new HandlerCollectionFactory(new EasyNetBusFactory.Log4NetLogger()));
            var rabbitMessagePublisher = new RabbitMessagePublisher(advancedBus, new MessageSerializer(), settings);
            var queueFactory = new QueueFactory(advancedBus);
            var rabbitQueueListener = new RabbitQueueListener(rabbitMessageConsumer, queueFactory, rabbitMessagePublisher);
            return rabbitQueueListener;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs exceptionArgs)
        {
            if (exceptionArgs == null || exceptionArgs.ExceptionObject == null) return;
            if (null != logger) logger.ErrorFormat("Caught unhandled exception: {0}", exceptionArgs.ExceptionObject);
        }
    }
}