using System;
using System.Collections.Generic;

namespace PowerPlant.Infrastructure.Services.AutoFillDataServices
{
    public class TimeRangeMemo
    {
        private Dictionary<(int, int), List<DateTime>> _memo = new Dictionary<(int, int), List<DateTime>>();

        /// <summary>
        /// Generate time range for year by 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="interval">Time interval in miliseconds</param>
        /// <returns></returns>
        public List<DateTime> GetRange(int year, int interval)
        {
            if (!_memo.ContainsKey((year, interval)))
            {
                var range = GenerateRange(year, interval);
                _memo.Add((year, interval), range);
            }

            return _memo[(year, interval)];
        }

        private List<DateTime> GenerateRange(int year, int interval)
        {
            var template = new List<DateTime>();
            var current = new DateTime(year, 1, 1, 0, 0, 0, 0);
            var to = new DateTime(year, 12, DateTime.DaysInMonth(year, 1), 23, 59, 59);

            while (current != to)
            {
                template.Add(current);
                current = current.AddMilliseconds(interval);
            }

            return template;
        }
    }
}
