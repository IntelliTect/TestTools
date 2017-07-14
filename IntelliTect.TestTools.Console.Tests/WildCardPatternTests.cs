using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntelliTect.TestTools.Console.Tests
{
    [TestClass]
    public class WildCardPatternTests
    {
        const string UnescapedString = @"*, [, ], ?, \";
        const string EscapedString = @"\*, \[, \], \?, \";

        [TestMethod]
        public void Escape_GivenUnescapedPattern_EscapeWildCards()
        {
            string escapedText = WildcardPattern.Escape(
                UnescapedString, '\\');
            Assert.AreEqual<string>(
                EscapedString, escapedText);

        }

        [TestMethod]
        public void Escape_GivenEscapedPattern_UnescapeWildCards()
        {
            string escapedText = WildcardPattern.Unescape(
                EscapedString, '\\');
            Assert.AreEqual<string>(
                UnescapedString, escapedText);
        }

    }
}