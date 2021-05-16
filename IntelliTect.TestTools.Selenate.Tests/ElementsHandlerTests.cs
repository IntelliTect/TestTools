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
        public void GetTextDoesNotThrowIfNotFound()
        {
            Assert.False(SetupMockedData().ContainsText("TestingA"));
        }

        private ElementsHandler SetupMockedData()
        {
            var mockElement1 = new Mock<IWebElement>();
            mockElement1.SetupGet(e1 => e1.Text).Returns("Testing1");
            var mockElement2 = new Mock<IWebElement>();
            mockElement2.SetupGet(e2 => e2.Text).Returns("Testing2");
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