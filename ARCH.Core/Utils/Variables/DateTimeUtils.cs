using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARCH.CoreLibrary.Utils.Variables
{
    public static class DateTimeUtils
    {
        private const int EveningEnds = 2;
        private const int MorningEnds = 12;
        private const int AfternoonEnds = 6;
        private static readonly DateTime Date1970 = new DateTime(1970, 1, 1);

        public static double UtcOffset
        {
            get { return DateTime.Now.Subtract(DateTime.UtcNow).TotalHours; }
        }

        public static int CalculateAge(this DateTime dateOfBirth)
        {
            return CalculateAge(dateOfBirth, DateTime.Now.Date);
        }

        public static int CalculateAge(this DateTime dateOfBirth, DateTime referenceDate)
        {
            var years = referenceDate.Year - dateOfBirth.Year;
            if (referenceDate.Month < dateOfBirth.Month || (referenceDate.Month == dateOfBirth.Month && referenceDate.Day < dateOfBirth.Day))
                --years;
            return years;
        }

        public static int GetCountDaysOfMonth(this DateTime date)
        {
            var nextMonth = date.AddMonths(1);
            return new DateTime(nextMonth.Year, nextMonth.Month, 1).AddDays(-1).Day;
        }

        public static DateTime GetFirstDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime GetFirstDayOfMonth(this DateTime date, DayOfWeek dayOfWeek)
        {
            var dt = date.GetFirstDayOfMonth();
            while (dt.DayOfWeek != dayOfWeek)
                dt = dt.AddDays(1);
            return dt;
        }

        public static DateTime GetLastDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, GetCountDaysOfMonth(date));
        }

        public static DateTime GetLastDayOfMonth(this DateTime date, DayOfWeek dayOfWeek)
        {
            var dt = date.GetLastDayOfMonth();
            while (dt.DayOfWeek != dayOfWeek)
                dt = dt.AddDays(-1);
            return dt;
        }

        public static bool IsToday(this DateTime dt)
        {
            return dt.Date == DateTime.Today;
        }

        public static DateTime SetTime(this DateTime date, int hours, int minutes, int seconds)
        {
            return date.SetTime(new TimeSpan(hours, minutes, seconds));
        }

        public static DateTime SetTime(this DateTime date, int hours, int minutes, int seconds, int milliseconds)
        {
            return date.SetTime(new TimeSpan(0, hours, minutes, seconds, milliseconds));
        }

        public static DateTime SetTime(this DateTime date, TimeSpan time)
        {
            return date.Date.Add(time);
        }

        public static DateTimeOffset ToDateTimeOffset(this DateTime localDateTime)
        {
            return localDateTime.ToDateTimeOffset(null);
        }

        public static DateTimeOffset ToDateTimeOffset(this DateTime localDateTime, TimeZoneInfo localTimeZone)
        {
            localTimeZone = (localTimeZone ?? TimeZoneInfo.Local);

            if (localDateTime.Kind != DateTimeKind.Unspecified)
                localDateTime = new DateTime(localDateTime.Ticks, DateTimeKind.Unspecified);

            return TimeZoneInfo.ConvertTimeToUtc(localDateTime, localTimeZone);
        }

        public static DateTime GetFirstDayOfWeek(this DateTime date)
        {
            return date.GetFirstDayOfWeek(CultureInfo.CurrentCulture);
        }

        public static DateTime GetFirstDayOfWeek(this DateTime date, CultureInfo cultureInfo)
        {
            cultureInfo = (cultureInfo ?? CultureInfo.CurrentCulture);

            var firstDayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            while (date.DayOfWeek != firstDayOfWeek)
                date = date.AddDays(-1);

            return date;
        }

        public static DateTime GetLastDayOfWeek(this DateTime date)
        {
            return date.GetLastDayOfWeek(CultureInfo.CurrentCulture);
        }

        public static DateTime GetLastDayOfWeek(this DateTime date, CultureInfo cultureInfo)
        {
            return date.GetFirstDayOfWeek(cultureInfo).AddDays(6);
        }

        public static DateTime GetWeeksWeekday(this DateTime date, DayOfWeek weekday)
        {
            return date.GetWeeksWeekday(weekday, CultureInfo.CurrentCulture);
        }

        public static DateTime GetWeeksWeekday(this DateTime date, DayOfWeek weekday, CultureInfo cultureInfo)
        {
            var firstDayOfWeek = date.GetFirstDayOfWeek(cultureInfo);
            return firstDayOfWeek.GetNextWeekday(weekday);
        }

        public static DateTime GetNextWeekday(this DateTime date, DayOfWeek weekday)
        {
            while (date.DayOfWeek != weekday)
                date = date.AddDays(1);
            return date;
        }

        public static DateTime GetPreviousWeekday(this DateTime date, DayOfWeek weekday)
        {
            while (date.DayOfWeek != weekday)
                date = date.AddDays(-1);
            return date;
        }

        public static bool IsDate(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return DateTime.TryParse(input, out DateTime dt);
            }
            else
            {
                return false;
            }
        }

        public static bool IsDateEqual(this DateTime date, DateTime dateToCompare)
        {
            return date.Date == dateToCompare.Date;
        }

        public static bool IsTimeEqual(this DateTime time, DateTime timeToCompare)
        {
            return time.TimeOfDay == timeToCompare.TimeOfDay;
        }

        public static long GetMillisecondsSince1970(this DateTime datetime)
        {
            var ts = datetime.Subtract(Date1970);
            return (long)ts.TotalMilliseconds;
        }

        public static long ToUnixEpoch(this DateTime dateTime)
        {
            return GetMillisecondsSince1970(dateTime);
        }

        public static bool IsWeekend(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        public static bool IsWeekend(this DayOfWeek d)
        {
            return !d.IsWeekday();
        }

        public static bool IsWeekday(this DayOfWeek d)
        {
            switch (d)
            {
                case DayOfWeek.Sunday:
                case DayOfWeek.Saturday: return false;

                default: return true;
            }
        }

        public static DateTime AddWeeks(this DateTime date, int value)
        {
            return date.AddDays(value * 7);
        }

        public static int GetDays(int year)
        {
            return GetDays(year, CultureInfo.CurrentCulture);
        }

        public static int GetDays(int year, CultureInfo culture)
        {
            var first = new DateTime(year, 1, 1, culture.Calendar);
            var last = new DateTime(year + 1, 1, 1, culture.Calendar);
            return GetDays(first, last);
        }

        public static int GetDays(this DateTime date)
        {
            return GetDays(date.Year, CultureInfo.CurrentCulture);
        }

        public static int GetDays(this DateTime date, CultureInfo culture)
        {
            return GetDays(date.Year, culture);
        }

        public static int GetDays(this DateTime fromDate, DateTime toDate)
        {
            return Convert.ToInt32(toDate.Subtract(fromDate).TotalDays);
        }

        public static string GetPeriodOfDay(this DateTime date)
        {
            var hour = date.Hour;
            if (hour < EveningEnds)
                return "evening";
            if (hour < MorningEnds)
                return "morning";
            return hour < AfternoonEnds ? "afternoon" : "evening";
        }

        public static int GetWeekOfYear(this DateTime dateTime, CultureInfo culture)
        {
            var calendar = culture.Calendar;
            var dateTimeFormat = culture.DateTimeFormat;

            return calendar.GetWeekOfYear(dateTime, dateTimeFormat.CalendarWeekRule, dateTimeFormat.FirstDayOfWeek);
        }

        public static int GetWeekOfYear(this DateTime dateTime)
        {
            return GetWeekOfYear(dateTime, CultureInfo.CurrentCulture);
        }

        public static bool IsEaster(this DateTime date)
        {
            int y = date.Year;
            int a = y % 19;
            int b = y / 100;
            int c = y % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = ((19 * a) + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + (2 * e) + (2 * i) - h - k) % 7;
            int m = (a + (11 * h) + (22 * l)) / 451;
            int month = (h + l - (7 * m) + 114) / 31;
            int day = ((h + l - (7 * m) + 114) % 31) + 1;

            DateTime dtEasterSunday = new DateTime(y, month, day);

            return date == dtEasterSunday;
        }

        public static bool IsBefore(this DateTime source, DateTime other)
        {
            return source.CompareTo(other) < 0;
        }

        public static bool IsAfter(this DateTime source, DateTime other)
        {
            return source.CompareTo(other) > 0;
        }

        public static DateTime Tomorrow(this DateTime date)
        {
            return date.AddDays(1);
        }

        public static DateTime Yesterday(this DateTime date)
        {
            return date.AddDays(-1);
        }

        public static string ToFriendlyDateString(this DateTime date, CultureInfo culture)
        {
            var sbFormattedDate = new StringBuilder();
            if (date.Date == DateTime.Today)
            {
                sbFormattedDate.Append("Today");
            }
            else if (date.Date == DateTime.Today.AddDays(-1))
            {
                sbFormattedDate.Append("Yesterday");
            }
            else if (date.Date > DateTime.Today.AddDays(-6))
            {
                // *** Show the Day of the week
                sbFormattedDate.Append(date.ToString("dddd").ToString(culture));
            }
            else
            {
                sbFormattedDate.Append(date.ToString("MMMM dd, yyyy").ToString(culture));
            }

            //append the time portion to the output
            sbFormattedDate.Append(" at ").Append(date.ToString("t").ToLower());
            return sbFormattedDate.ToString();
        }

        public static string ToFriendlyDateString(this DateTime date)
        {
            return ToFriendlyDateString(date, CultureInfo.CurrentCulture);
        }

        public static DateTime EndOfDay(this DateTime date)
        {
            return date.SetTime(23, 59, 59, 999);
        }

        public static DateTime Noon(this DateTime time)
        {
            return time.SetTime(12, 0, 0);
        }

        public static DateTime Midnight(this DateTime time)
        {
            return time.SetTime(0, 0, 0, 0);
        }

        public static bool IsWeekDay(this DateTime date)
        {
            return !date.IsWeekend();
        }

        public static string ToW3CDate(this DateTime dt)
        {
            return dt.ToUniversalTime().ToString("s") + "Z";
        }

        public static string TimeAgoString(this DateTime dateTime)
        {
            StringBuilder sb = new StringBuilder();
            TimeSpan timespan = DateTime.Now - dateTime;

            // A year or more?  Do "[Y] years and [M] months ago"
            if ((int)timespan.TotalDays >= 365)
            {
                // Years
                int nYears = (int)timespan.TotalDays / 365;
                sb.Append(nYears);
                if (nYears > 1)
                    sb.Append(" years");
                else
                    sb.Append(" year");

                // Months
                int remainingDays = (int)timespan.TotalDays - (nYears * 365);
                int nMonths = remainingDays / 30;
                if (nMonths == 1)
                    sb.Append(" and ").Append(nMonths).Append(" month");
                else if (nMonths > 1)
                    sb.Append(" and ").Append(nMonths).Append(" months");
            }
            // More than 60 days? (appx 2 months or 8 weeks)
            else if ((int)timespan.TotalDays >= 60)
            {
                // Do months
                int nMonths = (int)timespan.TotalDays / 30;
                sb.Append(nMonths).Append(" months");
            }
            // Weeks? (7 days or more)
            else if ((int)timespan.TotalDays >= 7)
            {
                int nWeeks = (int)timespan.TotalDays / 7;
                sb.Append(nWeeks);
                if (nWeeks == 1)
                    sb.Append(" week");
                else
                    sb.Append(" weeks");
            }
            // Days? (1 or more)
            else if ((int)timespan.TotalDays >= 1)
            {
                int nDays = (int)timespan.TotalDays;
                sb.Append(nDays);
                if (nDays == 1)
                    sb.Append(" day");
                else
                    sb.Append(" days");
            }
            // Hours?
            else if ((int)timespan.TotalHours >= 1)
            {
                int nHours = (int)timespan.TotalHours;
                sb.Append(nHours);
                if (nHours == 1)
                    sb.Append(" hour");
                else
                    sb.Append(" hours");
            }
            // Minutes?
            else if ((int)timespan.TotalMinutes >= 1)
            {
                int nMinutes = (int)timespan.TotalMinutes;
                sb.Append(nMinutes);
                if (nMinutes == 1)
                    sb.Append(" minute");
                else
                    sb.Append(" minutes");
            }
            // Seconds?
            else if ((int)timespan.TotalSeconds >= 1)
            {
                int nSeconds = (int)timespan.TotalSeconds;
                sb.Append(nSeconds);
                if (nSeconds == 1)
                    sb.Append(" second");
                else
                    sb.Append(" seconds");
            }
            // Just say "1 second" as the smallest unit of time
            else
            {
                sb.Append("1 second");
            }

            sb.Append(" ago");

            // For anything more than 6 months back, put " ([Month] [Year])" at the end, for better reference
            if ((int)timespan.TotalDays >= 30 * 6)
            {
                sb.AppendFormat(" ({0:MMMM} {1})", dateTime, dateTime.Year);
            }

            return sb.ToString();
        }

        public static bool IsToday(this DateTimeOffset dto)
        {
            return dto.Date.IsToday();
        }

        public static DateTimeOffset SetTime(this DateTimeOffset date, int hours, int minutes, int seconds)
        {
            return date.SetTime(new TimeSpan(hours, minutes, seconds));
        }

        public static DateTimeOffset SetTime(this DateTimeOffset date, TimeSpan time)
        {
            return date.SetTime(time, null);
        }

        public static DateTimeOffset SetTime(this DateTimeOffset date, TimeSpan time, TimeZoneInfo localTimeZone)
        {
            var localDate = date.ToLocalDateTime(localTimeZone);
            localDate.SetTime(time);
            return localDate.ToDateTimeOffset(localTimeZone);
        }

        public static System.DateTime ToLocalDateTime(this DateTimeOffset dateTimeUtc)
        {
            return dateTimeUtc.ToLocalDateTime(null);
        }

        public static System.DateTime ToLocalDateTime(this DateTimeOffset dateTimeUtc, TimeZoneInfo localTimeZone)
        {
            localTimeZone = (localTimeZone ?? TimeZoneInfo.Local);

            return TimeZoneInfo.ConvertTime(dateTimeUtc, localTimeZone).DateTime;
        }
    }
}
