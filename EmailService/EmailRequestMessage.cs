using System.Collections.Generic;

namespace EmailService
{
    public class EmailRequestedMessage
    {
        public string CorrelationId { get; set; }
        public string AppId { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}