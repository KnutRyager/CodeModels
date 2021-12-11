using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Common.Util.CollectionUtil;

namespace Common.Util
{
    public static class DateUtil
    {
        public static DateTime FirstOfMonth(int year, int month) => new(year, month, 1);

        public static DateTime Jan1(int year) => FirstOfMonth(year, 1);

        public static List<DateTime> HalfYearDates(int year) => MakeList(x => FirstOfMonth(year, 1 + x * 6), 0, 2);

        public static List<DateTime> QuarterDates(int year) => MakeList(x => FirstOfMonth(year, 1 + x * 3), 0, 4);

        public static List<DateTime> FirstOfMonths(int year) => MakeList(x => FirstOfMonth(year, 1 + x), 0, 12);

        public static List<DateTime> Yearly(DateTime startInclusive, DateTime endExclusive)
            => startInclusive >= endExclusive ? new List<DateTime>() : MakeList(x => startInclusive.AddYears(x), 0, IntervalYears(startInclusive, endExclusive));

        public static List<DateTime> HalfYearly(DateTime startInclusive, DateTime endExclusive)
            => startInclusive >= endExclusive ? new List<DateTime>() : MakeList(x => startInclusive.AddMonths(6 * x), 0, IntervalHalfYearly(startInclusive, endExclusive));

        public static List<DateTime> Quarterly(DateTime startInclusive, DateTime endExclusive)
            => startInclusive >= endExclusive ? new List<DateTime>() : MakeList(x => startInclusive.AddMonths(3 * x), 0, IntervalQuarterly(startInclusive, endExclusive));

        public static List<DateTime> Monthly(DateTime startInclusive, DateTime endExclusive)
            => startInclusive >= endExclusive ? new List<DateTime>() : MakeList(x => startInclusive.AddMonths(x), 0, IntervalMonths(startInclusive, endExclusive));

        public static int IntervalYears(DateTime? startInclusive, DateTime? endExclusive)
        {
            if (startInclusive == null || endExclusive == null) throw new ArgumentNullException($"Null in either startInclusive ({startInclusive}) or  endExclusive ({endExclusive}).");
            var sum = 0;
            while (startInclusive < endExclusive)
            {
                sum++;
                if (sum > 1000000) throw new ArgumentException("Possibly infinite loop in DateUtil.IntervalYears.");
                startInclusive = startInclusive.Value.AddYears(1);
            }
            return sum;
        }

        public static int IntervalHalfYearly(DateTime startInclusive, DateTime endExclusive)
            => IntervalMonths(startInclusive, endExclusive, 6);

        public static int IntervalQuarterly(DateTime startInclusive, DateTime endExclusive)
            => IntervalMonths(startInclusive, endExclusive, 3);

        public static int IntervalMonths(DateTime startInclusive, DateTime endExclusive)
            => IntervalMonths(startInclusive, endExclusive, 1);

        private static int IntervalMonths(DateTime? startInclusive, DateTime? endExclusive, int monthInterval)
        {
            if (startInclusive == null || endExclusive == null) throw new ArgumentNullException($"Null in either startInclusive ({startInclusive}) or  endExclusive ({endExclusive}).");
            var sum = 0;
            while (startInclusive < endExclusive)
            {
                sum++;
                if (sum > 1000000) throw new ArgumentException("Possibly infinite loop in DateUtil.IntervalMonths.");
                startInclusive = startInclusive.Value.AddMonths(monthInterval);
            }
            return sum;
        }

        public static int IntervalDays(DateTime startInclusive, DateTime endExclusive)
            => (int)(endExclusive - startInclusive).TotalDays;

        public static (DateTime startInclusive, DateTime endExclusive) GetBounds(IEnumerable<(DateTime startInclusive, DateTime endExclusive)> dates)
        {
            var starts = dates.Select(y => y.startInclusive);
            var ends = dates.Select(y => y.endExclusive);
            var min = starts.Min();
            var max = ends.Max();
            return (startInclusive: starts.FirstOrDefault(x => x == min), endExclusive: ends.FirstOrDefault(x => x == max));
        }

        private static readonly Regex _dateRegex = new("[0-9][0-9]-[0-9][0-9]-[0-9][0-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9]");

        public static DateTime? FindDate(string fileName)
        {
            var matched = _dateRegex.Match(fileName);
            if (matched.Success)
            {
                return DateTime.ParseExact(matched.Value, "MM-dd-yyyy-HH-mm", null);
            }
            return null;
        }
    }
}