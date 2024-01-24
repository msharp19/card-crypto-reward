using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Utilities
{
    public static class DateTimeUtility
    {
        /// <summary>
        /// Get the start and end dates of the previous month
        /// </summary>
        /// <returns></returns>
        public static (DateTime FromDate, DateTime ToDate) GetLastMonth()
        {
            // Get now - 1 month to get last month
            var lastMonth = DateTime.UtcNow.AddMonths(-1);

            // Build the start & end dates and return
            return (
                new DateTime(lastMonth.Year,lastMonth.Month, 1,0,0,0),
                new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0).AddSeconds(-1)
            );
        }
    }
}
