using System;
using EasyNetQ;
using log4net;

namespace RabbitWrapper
{
    public class EasyNetBusFactory
    {
        private readonly IQueueingSettings queueingSettings;

        public EasyNetBusFactory(IQueueingSettings queueingSettings)
        {
            this.queueingSettings = queueingSettings;
        }

        IBus CreateSimpleBus()
        {
            var logger = new Log4NetLogger();
            string connectionString = $"host={queueingSettings.RabbitHosts};username={queueingSettings.RabbitUserName};password={queueingSettings.RabbitPassword}";
            return RabbitHutch.CreateBus(connectionString, serviceRegister => serviceRegister.Register<IEasyNetQLogger>(_ => logger));
        }

        public IAdvancedBus CreateAdvancedBus()
        {
            return CreateSimpleBus().Advanced;
        }

        public class Log4NetLogger : IEasyNetQLogger
        {
            static readonly ILog Logger = LogManager.GetLogger("EasyNetQ");

            public void DebugWrite(string format, params object[] args)
            {
                Logger.DebugFormat(format, args);
            }

            public void InfoWrite(string format, params object[] args)
            {
                Logger.InfoFormat(format, args);
            }

            public void ErrorWrite(string format, params object[] args)
            {
                Logger.ErrorFormat(format, args);
            }

            public void ErrorWrite(Exception exception)
            {
                Logger.Error(exception);
            }
        }
    }
}