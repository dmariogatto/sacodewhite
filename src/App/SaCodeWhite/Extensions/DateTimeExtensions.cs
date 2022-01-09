using System;

namespace SaCodeWhite
{
    public static class DateTimeExtensions
    {
        public static bool InTheLastUtc(this DateTime dt, TimeSpan ts)
            => dt.ToUniversalTime() >= DateTime.UtcNow.Subtract(ts);
    }
}