using System;
using System.Globalization;
using System.Linq;

namespace GlobalSuite.Core.Helpers
{
    public static class DateHelper
    {
        public static bool IsAfter(this DateTime value, DateTime startDate)
        {
            return value > startDate;
        }

        public static bool IsBefore(this DateTime value, DateTime endDate)
        {
            return value < endDate;
        }

        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1, 0, 0, 0);
        }  
        public static DateTime LastDayOfMonth(this DateTime value)
        {
            
            return new DateTime(value.Year, value.Month+1,1, 23, 59, 59).AddDays(-1);
        } 
        public static DateTime FirstDayOfYear(this DateTime value)
        {
            return new DateTime(value.Year, 1, 1, 12, 0, 0);
        }

        public static DateTime LastDayOfYear(this DateTime value)
        {
            return new DateTime(value.Year, 12, 31, 23, 59, 59);
        } 
        public static DateTime ToExact(this DateTime value)
        {
            string[] validformats = new[] { "MM/dd/yyyy", "yyyy/MM/dd", "MM/dd/yyyy HH:mm:ss",
                "MM/dd/yyyy hh:mm tt","dd/MMM/yy HH:mm:ss tt", "dd-MMM-yy hh:mm:ss tt","dd-MMM-yy HH:mm:ss tt","yyyy-MM-dd HH:mm:ss",
               
                "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss, fff", 
            };
            var allPatterns = validformats.Union(CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns()).ToArray();
            IFormatProvider format = new CultureInfo("en-GB");
            var tryDtr = DateTime.TryParseExact($"{value}",allPatterns ,format, DateTimeStyles.AssumeLocal,out DateTime dtr);

            return tryDtr ? dtr : DateTime.MinValue;
        }

        public static string ToDate(this DateTime value)
        {
            return $"{value.ToExact().Date}".Split(' ')[0];
        }
    }



}
