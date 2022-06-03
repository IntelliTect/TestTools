using Moq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace IntelliTect.TestTools.Selenate.Tests
{
    public class ElementsHandlerTests
    {
        [Fact]
        public void GetTextReturnsExpectedWhenFound()
        {
            Assert.True(SetupMockedData().ContainsText("Testing1"));
        }

        [Fact]
        public void GetTextReturnsFalseWhenUnableToFindElementWithText()
        {
            Assert.False(SetupMockedData().ContainsText("TestingA"));
        }

        [Fact]
        public void GetSpecificExistingElementReturnsFoundElements()
        {
            Assert.NotNull(
                SetupMockedData()
                .GetSingleWebElement(x => 
                    x.Displayed));
        }

        [Fact]
        public void GetSpecificExistingElementThrowsWhenNoElementsMatch()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                SetupMockedData()
                .GetSingleWebElement(x => 
                    x.Text.Contains("Blaaaargh", StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public void GetElementsThrowsWhenNoElementsMatch()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                SetupMockedData()
                .GetSingleWebElement(x =>
                    x.Text.Contains("Blaaaargh", StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public void GetSpecificExistingElementThrowsWhenMultipleElementsMatch()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                SetupMockedData()
                .GetSingleWebElement(x =>
                    x.Text.Contains("Testing", StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public void GetElementsReturnsWhenMultipleElementsMatch()
        {
            Assert.Equal(2,
                SetupMockedData()
                .GetAllWebElements(x =>
                    x.Text.Contains("Testing", StringComparison.OrdinalIgnoreCase))
                .Count);
        }

        [Theory]
        [InlineData("blarg")] // Returns an empty IReadOnlyCollection with current mock
        [InlineData("null")] // Returns null. Note: this shouldn't be possible with vanilla selenium, but let's test for it just in case.
        public void GetSpecificExistingElementThrowsWhenNoElementsAreFound(string id)
        {
            Exception ex = Assert.Throws<WebDriverTimeoutException>(() =>
                SetupMockedData()
                .SetLocator(By.Id(id))
                .GetSingleWebElement(x =>
                    x.Text.Contains("Testing", StringComparison.OrdinalIgnoreCase)));

            Assert.NotNull(ex.InnerException);
            Assert.Equal(typeof(NoSuchElementException), ex.InnerException!.GetType());
        }

        private static ElementsHandler SetupMockedData()
        {
            var mockElement1 = new Mock<IWebElement>();
            mockElement1.SetupGet(e1 => e1.Text).Returns("Testing1");
            mockElement1.SetupGet(e1 => e1.Displayed).Returns(true);
            
            var mockElement2 = new Mock<IWebElement>();
            mockElement2.SetupGet(e2 => e2.Text).Returns("Testing2");
            mockElement2.SetupGet(e2 => e2.Displayed).Returns(false);
            var mockDriver = new Mock<IWebDriver>();
            mockDriver.Setup
                (f => f.FindElements(By.Id("test")))
                .Returns(
                    new ReadOnlyCollection<IWebElement>(
                        new List<IWebElement> { mockElement1.Object, mockElement2.Object }));

            mockDriver.Setup
                (f => f.FindElements(By.Id("blarg")))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>()));

            return new ElementsHandler(mockDriver.Object, By.Id("test"))
                .SetTimeout(TimeSpan.FromMilliseconds(20))
                .SetPollingIntervalMilliseconds(10);
        }
    }
}