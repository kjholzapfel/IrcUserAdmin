using System.Text.RegularExpressions;

namespace IrcUserAdmin.Tools
{
    public static class Extensions
    {
        private const string ValidIpAddressRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
        private const string ValidHostnameRegex = @"^(?=.{1,255}$)[0-9A-Za-z](?:(?:[0-9A-Za-z]|-){0,61}[0-9A-Za-z])?(?:\.[0-9A-Za-z](?:(?:[0-9A-Za-z]|-){0,61}[0-9A-Za-z])?)*\.?$";

        public static bool IsValidHostOrIp(this string host)
        {
            return Regex.IsMatch(host, ValidIpAddressRegex) || Regex.IsMatch(host, ValidHostnameRegex);
        }
    }
}