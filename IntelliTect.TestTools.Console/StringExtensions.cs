using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IntelliTect.TestTools.Console
{
    /// <summary>
    /// Useful string extensions for performing assertions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns true if the string matches the given pattern (case-insensitive).
        /// </summary>
        /// <param name="s">The string to match</param>
        /// <param name="pattern">The pattern to match it against.</param>
        /// <returns></returns>
        public static bool IsLikeRegEx(this string s, string pattern) =>
            new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(s);

        /// <summary>
        /// Implement's VB's Like operator logic.
        /// </summary>
        // Provided in addition to IsLike that takes an escape character 
        // even though a default escapeCharacter is provided as it
        // is hopefully simpler to use this one because no thinking 
        // about escapeCharacter is required.  
        public static bool IsLike(this string text, string pattern) =>
            new WildcardPattern(pattern).IsMatch(text);

        /// <summary>
        /// Implement's VB's Like operator logic.
        /// </summary>
        public static bool IsLike(this string text, string pattern, char escapeCharacter) =>
            new WildcardPattern(pattern, escapeCharacter).IsMatch(text);

        /// <summary>
        /// Converts a string of characters to a HashSet of characters. If the string
        /// contains character ranges, such as A-Z, all characters in the range are
        /// also added to the returned set of characters.
        /// </summary>
        /// <param name="charList">Character list string</param>
        private static HashSet<char> CharListToSet(string charList)
        {
            HashSet<char> set = new HashSet<char>();

            for (int i = 0; i < charList.Length; i++)
            {
                if ((i + 1) < charList.Length && charList[i + 1] == '-')
                {
                    // Character range
                    char startChar = charList[i++];
                    i++; // Hyphen
                    char endChar = (char)0;
                    if (i < charList.Length)
                        endChar = charList[i++];
                    for (int j = startChar; j <= endChar; j++)
                        set.Add((char)j);
                }
                else
                {
                    set.Add(charList[i]);
                }
            }
            return set;
        }
    }
}
