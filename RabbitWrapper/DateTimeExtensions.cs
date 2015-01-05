using System;

namespace RabbitWrapper
{
    public static class DateTimeExtensions
    {
        static readonly DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Unix time
        public static long SecondsSinceEpoch(this DateTime date)
        {
            var timeSinceEpoch = date.Subtract(EpochTime);
            return (long) timeSinceEpoch.TotalSeconds;
        }
    }
}