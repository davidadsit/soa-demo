using System;

namespace RabbitWrapper
{
    public class ExchangeName
    {
        public const int MaximumEventNameLength = 40;
        public readonly string EventName;
        public readonly int Version;

        public ExchangeName(string eventName, int version = 1)
        {
            if (eventName.Length > MaximumEventNameLength) throw new ArgumentException("Exchange names should not exceed 40 characters. Exchanges with names longer than 45 characters will not be tracked by the auditing tool.", "eventName");
            EventName = eventName.ToLowerInvariant();
            Version = version;
        }

        public override string ToString()
        {
            return string.Format("{0}.v{1}", EventName, Version);
        }
    }
}