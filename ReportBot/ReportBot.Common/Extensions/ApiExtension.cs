using System.Text;

namespace ReportBot.Common.Extensions
{
    public static class ApiExtension
    {
        public static string DecodeToken(this string token)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(token));
        }
    }
}
