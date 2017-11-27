using RabbitWrapper;

namespace PizzaCreationService
{
    public class Settings : IQueueingSettings
    {
        public string RabbitHosts => "rabbit";
        public string RabbitUserName => "guest";
        public string RabbitPassword => "guest";
        public string ExecutingApplication => "pizza-creation-service";
    }
}