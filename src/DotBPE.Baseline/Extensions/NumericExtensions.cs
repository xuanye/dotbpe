using System;

namespace DotBPE.Baseline.Extensions
{
    public static class NumericExtensions
    {
      
        public static TimeSpan Min(this TimeSpan source, TimeSpan other)
        {
            return source.Ticks > other.Ticks ? other : source;
        }

        public static TimeSpan Max(this TimeSpan source, TimeSpan other)
        {
            return source.Ticks < other.Ticks ? other : source;
        }

        public static TimeSpan Seconds(this int secondsValue)
        {
            return TimeSpan.FromSeconds(secondsValue);
        }
        public static TimeSpan Milliseconds(this int value)
        {
            return TimeSpan.FromMilliseconds(value);
        }

        public static TimeSpan Minutes(this int value)
        {
            return TimeSpan.FromMinutes(value);
        }

        public static TimeSpan Hours(this int value)
        {
            return TimeSpan.FromHours(value);
        }
    }
}
