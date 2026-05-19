using System;
using System.Text.RegularExpressions;

namespace Kyrsova_OOP.Services
{
    public static class TimeZoneResolver
    {
        private static readonly Regex OffsetPattern = new Regex(
            @"^(?:UTC|GMT)(?<sign>[+-])(?<hours>\d{1,2})(?::?(?<mins>\d{2}))?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static TimeZoneInfo Resolve(string? timeZoneId)
        {
            if (string.IsNullOrWhiteSpace(timeZoneId))
            {
                return TimeZoneInfo.Utc;
            }

            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }

            var match = OffsetPattern.Match(timeZoneId.Trim());
            if (match.Success)
            {
                var hours = ParseInt(match.Groups["hours"].Value);
                var minutes = match.Groups["mins"].Success
                    ? ParseInt(match.Groups["mins"].Value)
                    : 0;
                var sign = match.Groups["sign"].Value == "-" ? -1 : 1;
                var offset = new TimeSpan(sign * hours, sign * minutes, 0);
                var name = $"UTC{(sign >= 0 ? "+" : "-")}{Math.Abs(hours):00}:{Math.Abs(minutes):00}";
                return TimeZoneInfo.CreateCustomTimeZone(name, offset, name, name);
            }

            return TimeZoneInfo.Utc;
        }

        private static int ParseInt(string value)
        {
            if (int.TryParse(value, out var result))
            {
                return result;
            }

            return 0;
        }
    }
}
