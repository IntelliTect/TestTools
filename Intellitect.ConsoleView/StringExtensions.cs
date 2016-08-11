using System.Text.RegularExpressions;

namespace Intellitect.ConsoleView
{
    public static class StringExtensions
    {
        public static bool IsLike(this string s, string pattern)
        {
            return new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(s);
        }
    }
}
