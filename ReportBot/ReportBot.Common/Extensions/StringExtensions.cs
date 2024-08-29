using System.Globalization;

namespace ReportBot.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsDateString(this string self)
        {
            return DateTime.TryParseExact(self, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var _);
        }
    }
}
