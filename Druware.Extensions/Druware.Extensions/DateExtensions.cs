using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Druware.Extensions
{
    public static class DateExtensions
    {
        public static DateTime AdjustForNextBusinessDay(this DateTime originalDate)
        {
            DateTime adjustedDate = originalDate;

            // Weekends
            if (originalDate.DayOfWeek == DayOfWeek.Sunday)
            {
                adjustedDate = originalDate.AddDays(1);
            }
            if (originalDate.DayOfWeek == DayOfWeek.Saturday)
            {
                adjustedDate = originalDate.AddDays(2);
            }

            return adjustedDate;
        }
    }
}
