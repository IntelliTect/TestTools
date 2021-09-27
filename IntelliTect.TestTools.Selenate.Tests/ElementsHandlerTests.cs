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
                .GetSingleExistingElement(x => 
                    x.Displayed));
        }

        [Fact]
        public void GetSpecificExistingElementThrowsWhenNoElementsAreFound()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                SetupMockedData()
                .GetSingleExistingElement(x => 
                    x.Text.Contains("Blaaaargh", StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public void GetSpecificExistingElementThrowsWhenMultipleElementsAreFound()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                SetupMockedData()
                .GetSingleExistingElement(x =>
                    x.Text.Contains("Testing", StringComparison.OrdinalIgnoreCase)));
        }

        private static ElementsHandler SetupMockedData()
        {
            var mockElement1 = new Mock<IWebElement>();
            mockElement1.SetupGet(e1 => e1.Text).Returns("Testing1");
            mockElement1.SetupGet(e1 => e1.Displayed).Returns(true);
            var test = mockElement1.Object;
            var mockElement2 = new Mock<IWebElement>();
            mockElement2.SetupGet(e2 => e2.Text).Returns("Testing2");
            mockElement2.SetupGet(e2 => e2.Displayed).Returns(false);
            var mockDriver = new Mock<IWebDriver>();
            mockDriver.Setup
                (f => f.FindElements(It.IsAny<By>()))
                .Returns(
                    new ReadOnlyCollection<IWebElement>(
                        new List<IWebElement> { mockElement1.Object, mockElement2.Object }));

            return new ElementsHandler(mockDriver.Object, By.Id("test"))
                .SetTimeout(TimeSpan.FromMilliseconds(20))
                .SetPollingIntervalMilliseconds(10);
        }
    }
}