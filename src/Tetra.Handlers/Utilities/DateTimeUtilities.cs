using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Tetra.Handlers.Models;

namespace Tetra.Handlers.Utilities
{
    public class DateTimeUtilities
    {
        private static readonly IEnumerable<TimestampConfiguration> Configurations = new[]
        {
            new TimestampConfiguration
            {
                DateTimeFormat = "yyyyMMdd",
                RegexPattern = new Regex(@"\d\d\d\d\d\d\d\d")
            },
            new TimestampConfiguration
            {
                DateTimeFormat = "yyyy-MM-dd",
                RegexPattern = new Regex(@"\d\d\d\d-\d\d-\d\d")
            },
            new TimestampConfiguration
            {
                DateTimeFormat = "yyyy_MM_dd",
                RegexPattern = new Regex(@"\d\d\d\d_\d\d_\d\d")
            }
        };

        public static DateTime? ExtractDateTimeFromFilename(string filename)
        {
            foreach (var configuration in Configurations)
            {
                var timestamp = ExtractDateTimeFromFilename(filename, configuration);

                if (timestamp != null)
                {
                    return timestamp;
                }
            }

            return null;
        }

        private static DateTime? ExtractDateTimeFromFilename(
            string filename, 
            TimestampConfiguration configuration)
        {
            var match = configuration.RegexPattern.Match(filename);

            if (!match.Success)
            {
                return null;
            }

            var timestamp = filename.Substring(match.Index, match.Length);

            if (DateTime.TryParseExact(
                timestamp, configuration.DateTimeFormat, null, DateTimeStyles.None, out var result))
            {
                return result;
            }

            return null;
        }
    }
}
