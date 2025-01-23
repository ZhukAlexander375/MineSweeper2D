using System;

public static class DateTimeExtensions
{
    public static long ToUnixTimestamp(this DateTime dateTime)
    {
        return (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    public static DateTime FromUnixTimestamp(long timestamp)
    {
        return new DateTime(1970, 1, 1).AddSeconds(timestamp).ToLocalTime();
    }
}
