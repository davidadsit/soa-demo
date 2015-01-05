using RabbitWrapper;

namespace EmailService
{
    public class Settings : IQueueingSettings
    {
        public string RabbitHosts { get { return "ec2-54-149-117-183.us-west-2.compute.amazonaws.com"; } }
        public string RabbitUserName { get { return "craftsman"; } }
        public string RabbitPassword { get { return "utahsc2015"; } }
        public string ExecutingApplication { get { return "email-service"; } }
    }
}