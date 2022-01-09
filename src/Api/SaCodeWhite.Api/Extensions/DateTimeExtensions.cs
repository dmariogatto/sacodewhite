using System;

namespace SaCodeWhite.Api.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToEpochSeconds(this DateTime dateTime)
            => (long)(DateTime.UnixEpoch - dateTime).TotalSeconds;
    }
}
