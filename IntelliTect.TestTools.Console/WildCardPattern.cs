// This code was originally sourced from https://github.com/PowerShell/PowerShell/blob/main/src/System.Management.Automation/engine/regex.cs
// and then modified to remove of PowerShell specific elements.
#pragma warning disable 1634, 1691
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IntelliTect.TestTools.Console
{
    /// <summary>
    /// Provides enumerated values to use to set wildcard pattern
    /// matching options.
    /// </summary>
    [Flags]
    public enum WildcardOptions
    {
        /// <summary>
        /// Indicates that no special processing is required.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies that the wildcard pattern is compiled to an assembly.
        /// This yields faster execution but increases startup time.
        /// </summary>
        Compiled = 1,

        /// <summary>
        /// Specifies case-insensitive matching.
        /// </summary>
        IgnoreCase = 2,

        /// <summary>
        /// Specifies culture-invariant matching.
        /// </summary>
        CultureInvariant = 4
    };

    /// <summary>
    /// Represents a wildcard pattern.
    /// </summary>
    public sealed class WildcardPattern
    {
        //
        // char that escapes special chars
        //
        public char? EscapeCharacter { get; set; }

        //
        // we convert a wildcard pattern to a predicate
        //
        private Predicate<string> _isMatch;

        //
        // wildcard pattern
        //
        internal string Pattern { get; }

        //
        // options that control match behavior
        //
        internal WildcardOptions Options { get; } = WildcardOptions.None;

        /// <summary>
        /// wildcard pattern converted to regex pattern.
        /// </summary>
        internal string PatternConvertedToRegex
        {
            get
            {
                var patternRegex = WildcardPatternToRegexParser.Parse(this);
                return patternRegex.ToString();
            }
        }

        /// <summary>
        /// Initializes and instance of the WildcardPattern class
        /// for the specified wildcard pattern.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to match</param>
        /// <returns>The constructed WildcardPattern object</returns>
        /// <remarks> if wildCardType == None, the pattern does not have wild cards</remarks>
        public WildcardPattern(string pattern) :
            this(pattern, WildcardOptions.None) { }

        /// <summary>
        /// Initializes an instance of the WildcardPattern class for
        /// the specified wildcard pattern expression, with options
        /// that modify the pattern.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to match.</param>
        /// <param name="escapeCharacter">The escape character for the pattern.</param>
        /// <returns>The constructed WildcardPattern object</returns>
        /// <remarks> if wildCardType == None, the pattern does not have wild cards  </remarks>
        public WildcardPattern(string pattern, char escapeCharacter) :
            this(pattern, escapeCharacter, WildcardOptions.None)
        { }

        /// <summary>
        /// Initializes an instance of the WildcardPattern class for
        /// the specified wildcard pattern expression, with options
        /// that modify the pattern.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to match.</param>
        /// <param name="options">Wildcard options</param>
        /// <returns>The constructed WildcardPattern object</returns>
        /// <remarks> if wildCardType == None, the pattern does not have wild cards  </remarks>
        public WildcardPattern(string pattern, WildcardOptions options)
        {
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            Options = options;
        }

        /// <summary>
        /// Initializes an instance of the WildcardPattern class for
        /// the specified wildcard pattern expression, with options
        /// that modify the pattern.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to match.</param>
        /// <param name="escapeCharacter">The escape character for the pattern.</param>
        /// <param name="options">Wildcard options</param>
        /// <returns>The constructed WildcardPattern object</returns>
        /// <remarks> if wildCardType == None, the pattern does not have wild cards  </remarks>
        public WildcardPattern(string pattern, char escapeCharacter,
            WildcardOptions options = WildcardOptions.None) :
            this(pattern, options)
        {
            EscapeCharacter = escapeCharacter;

            bool previousCharacterWasEscape = false;
            foreach (char character in pattern)
            {
                if (character == EscapeCharacter)
                {
                    previousCharacterWasEscape = true;
                }
                else
                {
                    if (previousCharacterWasEscape)
                    {
                        if (!IsWildcardChar(character))
                        {
                            throw new ArgumentException(
                                $"{nameof(pattern)} contains escape characters, '{EscapeCharacter}', with non-wildcard characters.");
                        }
                    }
                    previousCharacterWasEscape = false;
                }
            }
        }

        private static readonly WildcardPattern s_matchAllIgnoreCasePattern = new WildcardPattern("*", WildcardOptions.None);

        /// <summary>
        /// Create a new WildcardPattern, or return an already created one.
        /// </summary>
        /// <param name="pattern">The pattern</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static WildcardPattern Get(string pattern, WildcardOptions options)
        {
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            if (pattern.Length == 1 && pattern[0] == '*')
                return s_matchAllIgnoreCasePattern;

            return new WildcardPattern(pattern, options);
        }

        /// <summary>
        /// Instantiate internal regex member if not already done.
        /// </summary>
        ///
        /// <returns> true on success, false otherwise </returns>
        ///
        /// <remarks>  </remarks>
        ///
        private void Init()
        {
            if (_isMatch == null)
            {
                if (Pattern.Length == 1 && Pattern[0] == '*')
                {
                    _isMatch = _ => true;
                }
                else
                {
                    var matcher = new WildcardPatternMatcher(this);
                    _isMatch = matcher.IsMatch;
                }
            }
        }

        /// <summary>
        /// Indicates whether the wildcard pattern specified in the WildcardPattern
        /// constructor finds a match in the input string.
        /// </summary>
        /// <param name="input">The string to search for a match.</param>
        /// <returns>true if the wildcard pattern finds a match; otherwise, false</returns>
        public bool IsMatch(string input)
        {
            Init();
            return input != null && _isMatch(input);
        }

        /// <summary>
        /// Escape special chars, except for those specified in <paramref name="charsNotToEscape"/>, in a string by replacing them with their escape codes.
        /// </summary>
        /// <param name="pattern">The input string containing the text to convert.</param>
        /// <param name="charsNotToEscape">Array of characters that not to escape</param>
        ///  <param name="escapeCharacter">The escape character</param>
        /// <returns>
        /// A string of characters with any metacharacters, except for those specified in <paramref name="charsNotToEscape"/>, converted to their escaped form.
        /// </returns>
        internal static string Escape(string pattern,
            char[] charsNotToEscape, char escapeCharacter)
        {
#pragma warning disable 56506

            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            if (charsNotToEscape == null)
            {
                throw new ArgumentNullException(nameof(charsNotToEscape));
            }

            char[] temp = new char[(pattern.Length * 2) + 1];
            int tempIndex = 0;

            for (int i = 0; i < pattern.Length; i++)
            {
                char ch = pattern[i];

                //
                // if it is a wildcard char, escape it
                //
                if (IsWildcardChar(ch) && !charsNotToEscape.Contains(ch))
                {
                    temp[tempIndex++] = escapeCharacter;
                }

                temp[tempIndex++] = ch;
            }

            if (tempIndex > 0)
            {
                return new string(temp, 0, tempIndex);
            }
            else
            {
                return String.Empty;
            }

#pragma warning restore 56506
        }

        /// <summary>
        /// Escape special chars in a string by replacing them with their escape codes.
        /// </summary>
        /// <param name="pattern">The input string containing the text to convert.</param>
        /// <param name="escapeCharacter">Allows for overriding the default escape character</param>
        /// <returns>
        /// A string of characters with any metacharacters converted to their escaped form.
        /// </returns>
        public static string Escape(
            string pattern, char escapeCharacter)
        {
            return Escape(pattern, new char[] { }, escapeCharacter);
        }

        /// <summary>
        /// Checks to see if the given string has any wild card characters in it.
        /// </summary>
        /// <param name="pattern">
        /// String which needs to be checked for the presence of wildcard chars
        /// </param>
        /// <param name="escapeCharacter">Allows for overriding the default escape character</param>
        /// <returns> true if the string has wild card chars, false otherwise. </returns>
        /// <remarks>
        /// Currently { '*', '?', '[', and ']' }, are considered wild card chars.
        /// To override the default escape character, specify the <paramref name="escapeCharacter"/> value.
        /// </remarks>
        public static bool ContainsWildcardCharacters(string pattern,
            char escapeCharacter)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return false;
            }

            bool result = false;

            for (int index = 0; index < pattern.Length; ++index)
            {
                if (IsWildcardChar(pattern[index]))
                {
                    result = true;
                    break;
                }

                // If it is an escape character then advance past
                // the next character

                if (pattern[index] == escapeCharacter)
                {
                    ++index;
                }
            }
            return result;
        }

        /// <summary>
        /// Unescapes any escaped characters in the input string.
        /// </summary>
        /// <param name="pattern">
        /// The input string containing the text to convert.
        /// </param>
        /// <param name="escapeCharacter">Allows for overriding the default escape character</param>
        /// <returns>
        /// A string of characters with any escaped characters
        /// converted to their unescaped form.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="pattern" /> is null.
        /// </exception>
        public static string Unescape(
            string pattern, char escapeCharacter)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            char[] temp = new char[pattern.Length];
            int tempIndex = 0;
            bool prevCharWasEscapeChar = false;

            for (int i = 0; i < pattern.Length; i++)
            {
                char ch = pattern[i];

                if (ch == escapeCharacter)
                {
                    if (prevCharWasEscapeChar)
                    {
                        temp[tempIndex++] = ch;
                        prevCharWasEscapeChar = false;
                    }
                    else
                    {
                        prevCharWasEscapeChar = true;
                    }
                    continue;
                }

                if (prevCharWasEscapeChar)
                {
                    if (!IsWildcardChar(ch))
                    {
                        temp[tempIndex++] = escapeCharacter;
                    }
                }

                temp[tempIndex++] = ch;
                prevCharWasEscapeChar = false;
            }

            // Need to account for a trailing escape character as a real
            // character

            if (prevCharWasEscapeChar)
            {
                temp[tempIndex++] = escapeCharacter;
                prevCharWasEscapeChar = false;
            }

            if (tempIndex > 0)
            {
                return new string(temp, 0, tempIndex);
            }
            else
            {
                return String.Empty;
            }
        } // Unescape

        public static bool IsWildcardChar(char ch)
        {
            return WildCardCharacters.Contains(ch);
        }

        public const string WildCardCharacters = "*?[]";
    }

    /// <summary>
    /// A base class for parsers of <see cref="WildcardPattern"/> patterns.
    /// </summary>
    internal abstract class WildcardPatternParser
    {
        /// <summary>
        /// Called from <see cref="Parse"/> method to indicate
        /// the beginning of the wildcard pattern.
        /// Default implementation simply returns.
        /// </summary>
        /// <param name="pattern">
        /// <see cref="WildcardPattern"/> object that includes both
        /// the text of the pattern (<see cref="WildcardPattern.Pattern"/>)
        /// and the pattern options (<see cref="WildcardPattern.Options"/>)
        /// </param>
        protected virtual void BeginWildcardPattern(WildcardPattern pattern)
        {
        }

        /// <summary>
        /// Called from <see cref="Parse"/> method to indicate that the next
        /// part of the pattern should match
        /// a literal character <paramref name="c"/>.
        /// </summary>
        protected abstract void AppendLiteralCharacter(char c);

        /// <summary>
        /// Called from <see cref="Parse"/> method to indicate that the next
        /// part of the pattern should match
        /// any string, including an empty string.
        /// </summary>
        protected abstract void AppendAsterix();

        /// <summary>
        /// Called from <see cref="Parse"/> method to indicate that the next
        /// part of the pattern should match
        /// any single character.
        /// </summary>
        protected abstract void AppendQuestionMark();

        /// <summary>
        /// Called from <see cref="Parse"/> method to indicate the end of the wildcard pattern.
        /// Default implementation simply returns.
        /// </summary>
        protected virtual void EndWildcardPattern()
        {
        }

        /// <summary>
        /// Called from <see cref="Parse"/> method to indicate
        /// the beginning of a bracket expression.
        /// </summary>
        /// <remarks>
        /// Bracket expressions of <see cref="WildcardPattern"/> are
        /// a greatly simplified version of bracket expressions of POSIX wildcards
        /// (http://www.opengroup.org/onlinepubs/9699919799/functions/fnmatch.html).
        /// Only literal characters and character ranges are supported.
        /// Negation (with either '!' or '^' characters),
        /// character classes ([:alpha:])
        /// and other advanced features are not supported.
        /// </remarks>
        protected abstract void BeginBracketExpression();

        /// <summary>
        /// Called from <see cref="Parse"/> method to indicate that the bracket expression
        /// should include a literal character <paramref name="c"/>.
        /// </summary>
        protected abstract void AppendLiteralCharacterToBracketExpression(char c);

        /// <summary>
        /// Called from <see cref="Parse"/> method to indicate that the bracket expression
        /// should include all characters from character range
        /// starting at <paramref name="startOfCharacterRange"/>
        /// and ending at <paramref name="endOfCharacterRange"/>
        /// </summary>
        protected abstract void AppendCharacterRangeToBracketExpression(
                        char startOfCharacterRange,
                        char endOfCharacterRange);

        /// <summary>
        /// Called from <see cref="Parse"/> method to indicate the end of a bracket expression.
        /// </summary>
        protected abstract void EndBracketExpression();

        /// <summary>
        /// PowerShell v1 and v2 treats all characters inside
        /// <paramref name="brackedExpressionContents"/> as literal characters,
        /// except '-' sign which denotes a range.  In particular it means that
        /// '^', '[', ']' are escaped within the bracket expression and don't
        /// have their regex-y meaning.
        /// </summary>
        /// <param name="brackedExpressionContents"></param>
        /// <param name="bracketExpressionOperators"></param>
        /// <param name="pattern"></param>
        /// <remarks>
        /// This method should be kept "internal"
        /// </remarks>
        internal void AppendBracketExpression(string brackedExpressionContents, string bracketExpressionOperators, string pattern)
        {
            this.BeginBracketExpression();

            int i = 0;
            while (i < brackedExpressionContents.Length)
            {
                if (((i + 2) < brackedExpressionContents.Length)
                    && (bracketExpressionOperators[i + 1] == '-'))
                {
                    char lowerBound = brackedExpressionContents[i];
                    char upperBound = brackedExpressionContents[i + 2];
                    i += 3;

                    if (lowerBound > upperBound)
                    {
                        throw NewWildcardPatternException(pattern);
                    }

                    this.AppendCharacterRangeToBracketExpression(lowerBound, upperBound);
                }
                else
                {
                    this.AppendLiteralCharacterToBracketExpression(brackedExpressionContents[i]);
                    i++;
                }
            }

            this.EndBracketExpression();
        }

        /// <summary>
        /// Parses <paramref name="pattern"/>, calling appropriate overloads
        /// in <paramref name="parser"/>
        /// </summary>
        /// <param name="pattern">Pattern to parse</param>
        /// <param name="parser">Parser to call back</param>
        public static void Parse(
            WildcardPattern pattern, WildcardPatternParser parser)
        {
            parser.BeginWildcardPattern(pattern);

            bool previousCharacterIsAnEscape = false;
            bool previousCharacterStartedBracketExpression = false;
            bool insideCharacterRange = false;
            StringBuilder characterRangeContents = null;
            StringBuilder characterRangeOperators = null;
            foreach (char c in pattern.Pattern)
            {
                if (insideCharacterRange)
                {
                    if (c == ']' && !previousCharacterStartedBracketExpression && !previousCharacterIsAnEscape)
                    {
                        // An unescaped closing square bracket closes the character set.  In other
                        // words, there are no nested square bracket expressions
                        // This is different than the POSIX spec
                        // (at http://www.opengroup.org/onlinepubs/9699919799/functions/fnmatch.html),
                        // but we are keeping this behavior for back-compatibility.

                        insideCharacterRange = false;
                        parser.AppendBracketExpression(characterRangeContents.ToString(), characterRangeOperators.ToString(), pattern.Pattern);
                        characterRangeContents = null;
                        characterRangeOperators = null;
                    }
                    else if (c != pattern.EscapeCharacter || previousCharacterIsAnEscape)
                    {
                        characterRangeContents.Append(c);
                        characterRangeOperators.Append((c == '-') && !previousCharacterIsAnEscape ? '-' : ' ');
                    }

                    previousCharacterStartedBracketExpression = false;
                }
                else
                {
                    if (c == '*' && !previousCharacterIsAnEscape)
                    {
                        parser.AppendAsterix();
                    }
                    else if (c == '?' && !previousCharacterIsAnEscape)
                    {
                        parser.AppendQuestionMark();
                    }
                    else if (c == '[' && !previousCharacterIsAnEscape)
                    {
                        insideCharacterRange = true;
                        characterRangeContents = new StringBuilder();
                        characterRangeOperators = new StringBuilder();
                        previousCharacterStartedBracketExpression = true;
                    }
                    else if (c != pattern.EscapeCharacter || previousCharacterIsAnEscape)
                    {
                        parser.AppendLiteralCharacter(c);
                    }
                }

                previousCharacterIsAnEscape = (c == pattern.EscapeCharacter) && (!previousCharacterIsAnEscape);
            }

            if (insideCharacterRange)
            {
                throw NewWildcardPatternException(pattern.Pattern);
            }

            if (previousCharacterIsAnEscape)
            {
                if (!pattern.Pattern.Equals($"{pattern.EscapeCharacter}", StringComparison.Ordinal)) // Win7 backcompatibility requires treating '`' pattern as '' pattern when this code was used with PowerShell.
                {
                    parser.AppendLiteralCharacter(pattern.Pattern[pattern.Pattern.Length - 1]);
                }
            }

            parser.EndWildcardPattern();
        }

        internal static Exception NewWildcardPatternException(string invalidPattern)
        {
            return new Exception(
                    $"The wildcard pattern, '{invalidPattern}', is invalid.");
        }
    };

    /// <summary>
    /// Convert a string with wild cards into its equivalent regex
    /// </summary>
    /// <remarks>
    /// A list of glob patterns and their equivalent regexes
    ///
    ///  glob pattern      regex
    /// -------------     -------
    /// *foo*              foo
    /// foo                ^foo$
    /// foo*bar            ^foo.*bar$
    /// foo`*bar           ^foo\*bar$
    ///
    /// for a more cases see the unit-test file RegexTest.cs
    /// </remarks>
    internal class WildcardPatternToRegexParser : WildcardPatternParser
    {
        private StringBuilder _regexPattern;
        private RegexOptions _regexOptions;

        private const string regexChars = "()[.?*{}^$+|\\"; // ']' is missing on purpose

        private static bool IsRegexChar(char ch)
        {
            for (int i = 0; i < regexChars.Length; i++)
            {
                if (ch == regexChars[i])
                {
                    return true;
                }
            }

            return false;
        }

        internal static RegexOptions TranslateWildcardOptionsIntoRegexOptions(WildcardOptions options)
        {
            RegexOptions regexOptions = RegexOptions.Singleline;

            if ((options & WildcardOptions.Compiled) != 0)
            {
                regexOptions |= RegexOptions.Compiled;
            }
            if ((options & WildcardOptions.IgnoreCase) != 0)
            {
                regexOptions |= RegexOptions.IgnoreCase;
            }
            if ((options & WildcardOptions.CultureInvariant) == WildcardOptions.CultureInvariant)
            {
                regexOptions |= RegexOptions.CultureInvariant;
            }

            return regexOptions;
        }

        protected override void BeginWildcardPattern(WildcardPattern pattern)
        {
            _regexPattern = new StringBuilder((pattern.Pattern.Length * 2) + 2);
            _regexPattern.Append('^');

            _regexOptions = TranslateWildcardOptionsIntoRegexOptions(pattern.Options);
        }

        internal static void AppendLiteralCharacter(StringBuilder regexPattern, char c)
        {
            if (IsRegexChar(c))
            {
                regexPattern.Append('\\');
            }
            regexPattern.Append(c);
        }

        protected override void AppendLiteralCharacter(char c)
        {
            AppendLiteralCharacter(_regexPattern, c);
        }

        protected override void AppendAsterix()
        {
            _regexPattern.Append(".*");
        }

        protected override void AppendQuestionMark()
        {
            _regexPattern.Append('.');
        }

        protected override void EndWildcardPattern()
        {
            _regexPattern.Append('$');

            // lines below are not strictly necessary and are included to preserve
            // wildcard->regex conversion from PS v1 (i.e. not to break unit tests
            // and not to break backcompatibility).
            string regexPatternString = _regexPattern.ToString();
            if (regexPatternString.Equals("^.*$", StringComparison.Ordinal))
            {
                _regexPattern.Remove(0, 4);
            }
            else
            {
                if (regexPatternString.StartsWith("^.*", StringComparison.Ordinal))
                {
                    _regexPattern.Remove(0, 3);
                }
                if (regexPatternString.EndsWith(".*$", StringComparison.Ordinal))
                {
                    _regexPattern.Remove(_regexPattern.Length - 3, 3);
                }
            }
        }

        protected override void BeginBracketExpression()
        {
            _regexPattern.Append('[');
        }

        internal static void AppendLiteralCharacterToBracketExpression(StringBuilder regexPattern, char c)
        {
            if (c == '[')
            {
                regexPattern.Append('[');
            }
            else if (c == ']')
            {
                regexPattern.Append(@"\]");
            }
            else if (c == '-')
            {
                regexPattern.Append(@"\x2d");
            }
            else
            {
                AppendLiteralCharacter(regexPattern, c);
            }
        }

        protected override void AppendLiteralCharacterToBracketExpression(char c)
        {
            AppendLiteralCharacterToBracketExpression(_regexPattern, c);
        }

        internal static void AppendCharacterRangeToBracketExpression(
                        StringBuilder regexPattern,
                        char startOfCharacterRange,
                        char endOfCharacterRange)
        {
            AppendLiteralCharacterToBracketExpression(regexPattern, startOfCharacterRange);
            regexPattern.Append('-');
            AppendLiteralCharacterToBracketExpression(regexPattern, endOfCharacterRange);
        }

        protected override void AppendCharacterRangeToBracketExpression(
                        char startOfCharacterRange,
                        char endOfCharacterRange)
        {
            AppendCharacterRangeToBracketExpression(_regexPattern, startOfCharacterRange, endOfCharacterRange);
        }

        protected override void EndBracketExpression()
        {
            _regexPattern.Append(']');
        }

        /// <summary>
        /// Parses a <paramref name="wildcardPattern"/> into a <see cref="Regex"/>
        /// </summary>
        /// <param name="wildcardPattern">Wildcard pattern to parse</param>
        /// <returns>Regular expression equivalent to <paramref name="wildcardPattern"/></returns>
        public static Regex Parse(WildcardPattern wildcardPattern)
        {
            WildcardPatternToRegexParser parser = new WildcardPatternToRegexParser();
            WildcardPatternParser.Parse(wildcardPattern, parser);
            try
            {
                return new Regex(parser._regexPattern.ToString(), parser._regexOptions);
            }
            catch (ArgumentException)
            {
                throw WildcardPatternParser.NewWildcardPatternException(wildcardPattern.Pattern);
            }
        }
    }

    internal class WildcardPatternMatcher
    {
        private readonly PatternElement[] _patternElements;
        private readonly CharacterNormalizer _characterNormalizer;

        internal WildcardPatternMatcher(WildcardPattern wildcardPattern)
        {
            _characterNormalizer = new CharacterNormalizer(wildcardPattern.Options);
            _patternElements = MyWildcardPatternParser.Parse(
                            wildcardPattern,
                            _characterNormalizer);
        }

        internal bool IsMatch(string str)
        {
            // - each state of NFA is represented by (patternPosition, stringPosition) tuple
            //     - state transitions are documented in
            //       ProcessStringCharacter and ProcessEndOfString methods
            // - the algorithm below tries to see if there is a path
            //   from (0, 0) to (lengthOfPattern, lengthOfString)
            //    - this is a regular graph traversal
            //    - there are O(1) edges per node (at most 2 edges)
            //      so the whole graph traversal takes O(number of nodes in the graph) =
            //      = O(lengthOfPattern * lengthOfString) time
            //    - for efficient remembering which states have already been visited,
            //      the traversal goes methodically from beginning to end of the string
            //      therefore requiring only O(lengthOfPattern) memory for remembering
            //      which states have been already visited
            //  - Wikipedia calls this algorithm the "NFA" algorithm at
            //    http://en.wikipedia.org/wiki/Regular_expression#Implementations_and_running_times

            var patternPositionsForCurrentStringPosition =
                    new PatternPositionsVisitor(_patternElements.Length);
            patternPositionsForCurrentStringPosition.Add(0);

            var patternPositionsForNextStringPosition =
                    new PatternPositionsVisitor(_patternElements.Length);

            for (int currentStringPosition = 0;
                 currentStringPosition < str.Length;
                 currentStringPosition++)
            {
                char currentStringCharacter = _characterNormalizer.Normalize(str[currentStringPosition]);
                patternPositionsForCurrentStringPosition.StringPosition = currentStringPosition;
                patternPositionsForNextStringPosition.StringPosition = currentStringPosition + 1;

                while (patternPositionsForCurrentStringPosition.MoveNext(out int patternPosition))
                {
                    _patternElements[patternPosition].ProcessStringCharacter(
                        currentStringCharacter,
                        patternPosition,
                        patternPositionsForCurrentStringPosition,
                        patternPositionsForNextStringPosition);
                }

                // swap patternPositionsForCurrentStringPosition
                // with patternPositionsForNextStringPosition
                var tmp = patternPositionsForCurrentStringPosition;
                patternPositionsForCurrentStringPosition = patternPositionsForNextStringPosition;
                patternPositionsForNextStringPosition = tmp;
            }

            while (patternPositionsForCurrentStringPosition.MoveNext(out int patternPosition2))
            {
                _patternElements[patternPosition2].ProcessEndOfString(
                    patternPosition2,
                    patternPositionsForCurrentStringPosition);
            }

            return patternPositionsForCurrentStringPosition.ReachedEndOfPattern;
        }

        private class PatternPositionsVisitor
        {
            private readonly int _lengthOfPattern;

            private readonly int[] _isPatternPositionVisitedMarker;

            private readonly int[] _patternPositionsForFurtherProcessing;
            private int _patternPositionsForFurtherProcessingCount;

            public PatternPositionsVisitor(int lengthOfPattern)
            {
                if (lengthOfPattern < 0)
                {
                    throw new ArgumentException(
                                $"Caller should verify {nameof(lengthOfPattern)} >= 0", nameof(lengthOfPattern));
                }

                _lengthOfPattern = lengthOfPattern;

                _isPatternPositionVisitedMarker = new int[lengthOfPattern + 1];
                for (int i = 0; i < _isPatternPositionVisitedMarker.Length; i++)
                {
                    _isPatternPositionVisitedMarker[i] = -1;
                }

                _patternPositionsForFurtherProcessing = new int[lengthOfPattern];
                _patternPositionsForFurtherProcessingCount = 0;
            }

            public int StringPosition { private get; set; }

            public void Add(int patternPosition)
            {
                if (patternPosition < 0)
                {
                    throw new ArgumentException(
                            "There should never be more elements in the queue than the length of the pattern", nameof(patternPosition));
                }
                if (patternPosition > _lengthOfPattern)
                {
                    throw new ArgumentException(
                            $"Caller should verify {nameof(patternPosition)} <= this.{nameof(_lengthOfPattern)}");
                }

                // is patternPosition already visited?);
                if (_isPatternPositionVisitedMarker[patternPosition] == this.StringPosition)
                {
                    return;
                }

                // mark patternPosition as visited
                _isPatternPositionVisitedMarker[patternPosition] = this.StringPosition;

                // add patternPosition to the queue for further processing
                if (patternPosition < _lengthOfPattern)
                {
                    _patternPositionsForFurtherProcessing[_patternPositionsForFurtherProcessingCount] = patternPosition;
                    _patternPositionsForFurtherProcessingCount++;
                    if (_patternPositionsForFurtherProcessingCount > _lengthOfPattern)
                    {
                        throw new InvalidOperationException(
                                "There should never be more elements in the queue than the length of the pattern");
                    }
                }
            }

            public bool ReachedEndOfPattern
            {
                get
                {
                    return _isPatternPositionVisitedMarker[_lengthOfPattern] >= this.StringPosition;
                }
            }

            // non-virtual MoveNext is more performant
            // than implementing IEnumerable / virtual MoveNext
            public bool MoveNext(out int patternPosition)
            {
                if (_patternPositionsForFurtherProcessingCount < 0) {
                    throw new InvalidOperationException(
                            "There should never be more elements in the queue than the length of the pattern");
                }
                if (_patternPositionsForFurtherProcessingCount == 0)
                {
                    patternPosition = -1;
                    return false;
                }

                _patternPositionsForFurtherProcessingCount--;
                patternPosition = _patternPositionsForFurtherProcessing[_patternPositionsForFurtherProcessingCount];
                return true;
            }
        }

        private abstract class PatternElement
        {
            public abstract void ProcessStringCharacter(
                            char currentStringCharacter,
                            int currentPatternPosition,
                            PatternPositionsVisitor patternPositionsForCurrentStringPosition,
                            PatternPositionsVisitor patternPositionsForNextStringPosition);

            public abstract void ProcessEndOfString(
                            int currentPatternPosition,
                            PatternPositionsVisitor patternPositionsForEndOfStringPosition);
        }

        private class QuestionMarkElement : PatternElement
        {
            public override void ProcessStringCharacter(
                            char currentStringCharacter,
                            int currentPatternPosition,
                            PatternPositionsVisitor patternPositionsForCurrentStringPosition,
                            PatternPositionsVisitor patternPositionsForNextStringPosition)
            {
                // '?' : (patternPosition, stringPosition) => (patternPosition + 1, stringPosition + 1)
                patternPositionsForNextStringPosition.Add(currentPatternPosition + 1);
            }

            public override void ProcessEndOfString(
                            int currentPatternPosition,
                            PatternPositionsVisitor patternPositionsForEndOfStringPosition)
            {
                // '?' : (patternPosition, endOfString) => <no transitions out of this state - cannot move beyond end of string>
            }
        }

        private class LiteralCharacterElement : QuestionMarkElement
        {
            private readonly char _literalCharacter;

            public LiteralCharacterElement(char literalCharacter)
            {
                _literalCharacter = literalCharacter;
            }

            public override void ProcessStringCharacter(
                            char currentStringCharacter,
                            int currentPatternPosition,
                            PatternPositionsVisitor patternPositionsForCurrentStringPosition,
                            PatternPositionsVisitor patternPositionsForNextStringPosition)
            {
                if (_literalCharacter == currentStringCharacter)
                {
                    base.ProcessStringCharacter(
                            currentStringCharacter,
                            currentPatternPosition,
                            patternPositionsForCurrentStringPosition,
                            patternPositionsForNextStringPosition);
                }
            }
        }

        private class BracketExpressionElement : QuestionMarkElement
        {
            private readonly Regex _Regex;

            public BracketExpressionElement(Regex regex)
            {
                _Regex = regex ?? throw new ArgumentNullException(nameof(regex));
            }

            public override void ProcessStringCharacter(
                            char currentStringCharacter,
                            int currentPatternPosition,
                            PatternPositionsVisitor patternPositionsForCurrentStringPosition,
                            PatternPositionsVisitor patternPositionsForNextStringPosition)
            {
                if (_Regex.IsMatch(new string(currentStringCharacter, 1)))
                {
                    base.ProcessStringCharacter(currentStringCharacter, currentPatternPosition,
                                                patternPositionsForCurrentStringPosition,
                                                patternPositionsForNextStringPosition);
                }
            }
        }

        private class AsterixElement : PatternElement
        {
            public override void ProcessStringCharacter(
                            char currentStringCharacter,
                            int currentPatternPosition,
                            PatternPositionsVisitor patternPositionsForCurrentStringPosition,
                            PatternPositionsVisitor patternPositionsForNextStringPosition)
            {
                // '*' : (patternPosition, stringPosition) => (patternPosition + 1, stringPosition)
                patternPositionsForCurrentStringPosition.Add(currentPatternPosition + 1);

                // '*' : (patternPosition, stringPosition) => (patternPosition, stringPosition + 1)
                patternPositionsForNextStringPosition.Add(currentPatternPosition);
            }

            public override void ProcessEndOfString(
                            int currentPatternPosition,
                            PatternPositionsVisitor patternPositionsForEndOfStringPosition)
            {
                // '*' : (patternPosition, endOfString) => (patternPosition + 1, endOfString)
                patternPositionsForEndOfStringPosition.Add(currentPatternPosition + 1);
            }
        }

        private class MyWildcardPatternParser : WildcardPatternParser
        {
            private readonly List<PatternElement> _patternElements = new List<PatternElement>();
            private CharacterNormalizer _characterNormalizer;
            private RegexOptions _regexOptions;
            private StringBuilder _bracketExpressionBuilder;

            public static PatternElement[] Parse(
                            WildcardPattern pattern,
                            CharacterNormalizer characterNormalizer)
            {
                var parser = new MyWildcardPatternParser
                {
                    _characterNormalizer = characterNormalizer,
                    _regexOptions = WildcardPatternToRegexParser.TranslateWildcardOptionsIntoRegexOptions(pattern.Options),
                };
                WildcardPatternParser.Parse(pattern, parser);
                return parser._patternElements.ToArray();
            }

            protected override void AppendLiteralCharacter(char c)
            {
                c = _characterNormalizer.Normalize(c);
                _patternElements.Add(new LiteralCharacterElement(c));
            }

            protected override void AppendAsterix()
            {
                _patternElements.Add(new AsterixElement());
            }

            protected override void AppendQuestionMark()
            {
                _patternElements.Add(new QuestionMarkElement());
            }

            protected override void BeginBracketExpression()
            {
                _bracketExpressionBuilder = new StringBuilder();
                _bracketExpressionBuilder.Append('[');
            }

            protected override void AppendLiteralCharacterToBracketExpression(char c)
            {
                WildcardPatternToRegexParser.AppendLiteralCharacterToBracketExpression(
                    _bracketExpressionBuilder,
                    c);
            }

            protected override void AppendCharacterRangeToBracketExpression(
                            char startOfCharacterRange,
                            char endOfCharacterRange)
            {
                WildcardPatternToRegexParser.AppendCharacterRangeToBracketExpression(
                    _bracketExpressionBuilder,
                    startOfCharacterRange,
                    endOfCharacterRange);
            }

            protected override void EndBracketExpression()
            {
                _bracketExpressionBuilder.Append(']');
                Regex regex = new Regex(_bracketExpressionBuilder.ToString(), _regexOptions);
                _patternElements.Add(new BracketExpressionElement(regex));
            }
        }

        private struct CharacterNormalizer
        {
            private readonly CultureInfo _cultureInfo;
            private readonly bool _caseInsensitive;

            public CharacterNormalizer(WildcardOptions options)
            {
                _caseInsensitive = 0 != (options & WildcardOptions.IgnoreCase);
                if (_caseInsensitive)
                {
                    _cultureInfo = 0 != (options & WildcardOptions.CultureInvariant)
                        ? CultureInfo.InvariantCulture
                        : CultureInfo.CurrentCulture;
                }
                else
                {
                    // Don't bother saving the culture if we won't use it
                    _cultureInfo = null;
                }
            }

            public char Normalize(char x)
            {
                if (_caseInsensitive)
                {
                    return _cultureInfo.TextInfo.ToLower(x);
                }

                return x;
            }
        }
    }

    /// <summary>
    /// Translates a <see cref="WildcardPattern"/> into a DOS wildcard
    /// </summary>
    internal class WildcardPatternToDosWildcardParser : WildcardPatternParser
    {
        private readonly StringBuilder _result = new StringBuilder();

        protected override void AppendLiteralCharacter(char c)
        {
            _result.Append(c);
        }

        protected override void AppendAsterix()
        {
            _result.Append('*');
        }

        protected override void AppendQuestionMark()
        {
            _result.Append('?');
        }

        protected override void BeginBracketExpression()
        {
        }

        protected override void AppendLiteralCharacterToBracketExpression(char c)
        {
        }

        protected override void AppendCharacterRangeToBracketExpression(char startOfCharacterRange, char endOfCharacterRange)
        {
        }

        protected override void EndBracketExpression()
        {
            _result.Append('?');
        }

        /// <summary>
        /// Converts <paramref name="wildcardPattern"/> into a DOS wildcard
        /// </summary>
        internal static string Parse(WildcardPattern wildcardPattern)
        {
            var parser = new WildcardPatternToDosWildcardParser();
            WildcardPatternParser.Parse(wildcardPattern, parser);
            return parser._result.ToString();
        }
    }
}

