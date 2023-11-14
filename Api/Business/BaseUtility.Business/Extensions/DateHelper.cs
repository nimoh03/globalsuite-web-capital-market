using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseUtility.Business
{
    public static class DateHelper
    {
        public static DateTime ToDate(this string s)
        {
            string[] validformats = new[] { "MM/dd/yyyy", "yyyy/MM/dd", "MM/dd/yyyy HH:mm:ss",
                                        "MM/dd/yyyy hh:mm tt","dd/MMM/yy HH:mm:ss tt","dd-MMM-yy HH:mm:ss tt","yyyy-MM-dd HH:mm:ss",
                                        "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss, fff", 
                };
           var allPatterns = validformats.Union(CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns()).ToArray();
            IFormatProvider formatProvider = CultureInfo.CurrentCulture;
            var tryDtr = DateTime.TryParseExact(s,allPatterns ,formatProvider, DateTimeStyles.AssumeLocal,out DateTime dtr);
            return tryDtr ? dtr.Date : DateTime.MinValue;
        }
    }
}
