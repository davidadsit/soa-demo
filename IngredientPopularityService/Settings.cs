using RabbitWrapper;

namespace IngredientPopularityService
{
    public class Settings : IQueueingSettings
    {
        public string RabbitHosts => "rabbit";
        public string RabbitUserName => "guest";
        public string RabbitPassword => "guest";
        public string ExecutingApplication => "ingredient-popularity-service";
    }
}