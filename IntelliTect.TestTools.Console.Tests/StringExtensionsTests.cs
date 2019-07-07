using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntelliTect.TestTools.Console.Tests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void IsLike_GivenLikeString_ReturnsTrue()
        {
            Assert.IsTrue("ThisIsATest".IsLike("ThisIsATest"));
            Assert.IsTrue("ThisIsATest".IsLike("This*Test"));
            Assert.IsTrue("ThisIsTestNumber 3".IsLike("ThisIsTestNumber* 3"));
            Assert.IsTrue("ThisIsATest".IsLike("ThisIsATest"));
            Assert.IsTrue("ThisIsATest".IsLike("ThisIsATest"));
            Assert.IsTrue("ThisIsATest".IsLike("ThisIsATest"));
        }

        [TestMethod]
        public void IsLike_GivenLikeStringWithSpaces_ReturnsTrue()
        {
            const string output = @"*  3";

            Assert.IsTrue(output.IsLike("*3"), "First");
            Assert.IsTrue(output.IsLike("* 3"), "Second");
        }

        [TestMethod]
        public void IsLike_GivenLikeStringWithEscape_ReturnsTrue()
        {
            const string output = @"*3";

            Assert.IsTrue(output.IsLike("\\*3", '\\'));
        }

        [TestMethod]
        public void IsLike_GivenLikeStringWithOverrideEscape_ReturnsTrue()
        {
            const string output = @"*3";

            Assert.IsTrue(output.IsLike("`*3",'`'));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IsLike_GivenInvalideEscapeCharacter_Throws()
        {
            const string output = @"*3";

            Assert.IsTrue(output.IsLike(@"\3", '\\'));
        }
    }
}