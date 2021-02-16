using Moq;
using OpenQA.Selenium;
using System;
using Xunit;

namespace IntelliTect.TestTools.Selenate.Tests
{
    public class ElementHandlerTests
    {
        [Fact]
        public void ClickDoesNotThrowIfSuccessful()
        {
            var mockElement = new Mock<IWebElement>();
            mockElement
                .Setup(c => c.Click())
                .Verifiable();
            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .Setup(f => f.FindElement(It.IsAny<By>()))
                .Returns(mockElement.Object);

            var element = SetupElementHandler(mockDriver.Object);
            element.Click();

            mockElement.Verify();
        }

        [Fact]
        public void ClickThrowsIfUnsuccessful()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .Setup(f => f.FindElement(It.IsAny<By>()))
                .Throws<NoSuchElementException>();

            var element = SetupElementHandler(mockDriver.Object);

            Assert.Throws<WebDriverTimeoutException>(() => element.Click());
        }

        [Fact]
        public void SendTextDoesNotThrowIfSuccessful()
        {
            var mockElement = new Mock<IWebElement>();
            mockElement
                .Setup(c => c.SendKeys(It.IsAny<string>()))
                .Verifiable();
            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .Setup(f => f.FindElement(It.IsAny<By>()))
                .Returns(mockElement.Object);

            var element = SetupElementHandler(mockDriver.Object);
            element.SendKeys("Hello");

            mockElement.Verify(d => d.SendKeys(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void SendTextThrowsIfUnsuccessful()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .Setup(f => f.FindElement(It.IsAny<By>()))
                .Throws<NoSuchElementException>();

            var element = SetupElementHandler(mockDriver.Object);

            Assert.Throws<WebDriverTimeoutException>(() => element.SendKeys("Hello"));
        }

        [Fact]
        public void ClearDoesNotThrowIfSuccessful()
        {
            var mockElement = new Mock<IWebElement>();
            mockElement
                .Setup(c => c.Clear())
                .Verifiable();
            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .Setup(f => f.FindElement(It.IsAny<By>()))
                .Returns(mockElement.Object);

            var element = SetupElementHandler(mockDriver.Object);
            element.Clear();

            mockElement.Verify(d => d.Clear(), Times.Once);
        }

        [Fact]
        public void ClearThrowsIfUnsuccessful()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .Setup(f => f.FindElement(It.IsAny<By>()))
                .Throws<NoSuchElementException>();

            var element = SetupElementHandler(mockDriver.Object);

            Assert.Throws<WebDriverTimeoutException>(() => element.Clear());
        }

        [Fact]
        public void WaitForDisplayedReturnsTrueIfSuccessful()
        {
            var mockElement = new Mock<IWebElement>();
            mockElement
                .Setup(c => c.Displayed)
                .Returns(true);

            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .Setup(f => f.FindElement(It.IsAny<By>()))
                .Returns(mockElement.Object);

            var element = SetupElementHandler(mockDriver.Object);
            
            Assert.True(element.WaitForDisplayed());
        }

        [Fact]
        public void WaitForDisplayedReturnsFalseIfUnsuccessful()
        {
            var mockElement = new Mock<IWebElement>();
            mockElement
                .Setup(c => c.Displayed)
                .Returns(false);

            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .Setup(f => f.FindElement(It.IsAny<By>()))
                .Returns(mockElement.Object);

            var element = SetupElementHandler(mockDriver.Object);

            Assert.False(element.WaitForDisplayed());
        }

        [Fact]
        public void WaitForDisplayedReturnsFalseIfNoElement()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .Setup(f => f.FindElement(It.IsAny<By>()))
                .Throws<NoSuchElementException>();

            var element = SetupElementHandler(mockDriver.Object);

            Assert.False(element.WaitForDisplayed());
        }

        [Fact]
        public void WaitForNotDisplayedReturnsTrueIfSuccessful()
        {
            var mockElement = new Mock<IWebElement>();
            mockElement
                .Setup(c => c.Displayed)
                .Returns(false);

            var mockDriver = new Mock<IWebDriver>();
            mockDriver.Setup
                (f => f.FindElement(It.IsAny<By>()))
                .Returns(mockElement.Object);

            var element = SetupElementHandler(mockDriver.Object);

            Assert.True(element.WaitForNotDisplayed());
        }

        [Fact]
        public void WaitForNotDisplayedReturnsTrueIfNoElement()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .Setup(f => f.FindElement(It.IsAny<By>()))
                .Throws<NoSuchElementException>();

            var element = SetupElementHandler(mockDriver.Object);

            Assert.True(element.WaitForNotDisplayed());
        }

        [Fact]
        public void WaitForNotDisplayedReturnsFalseUnsuccessful()
        {
            var mockElement = new Mock<IWebElement>();
            mockElement
                .Setup(c => c.Displayed)
                .Returns(true);

            var mockDriver = new Mock<IWebDriver>();
            mockDriver.Setup
                (f => f.FindElement(It.IsAny<By>()))
                .Returns(mockElement.Object);

            var element = SetupElementHandler(mockDriver.Object);

            Assert.False(element.WaitForNotDisplayed());
        }

        [Fact]
        public void WaitForEnabledReturnsTrueIfSuccessful()
        {
            var mockElement = new Mock<IWebElement>();
            mockElement
                .Setup(c => c.Enabled)
                .Returns(true);

            var mockDriver = new Mock<IWebDriver>();
            mockDriver.Setup
                (f => f.FindElement(It.IsAny<By>()))
                .Returns(mockElement.Object);

            var element = SetupElementHandler(mockDriver.Object);

            Assert.True(element.WaitForEnabled());
        }

        [Fact]
        public void WaitForEnabledReturnsFalseIfUnsuccessful()
        {
            var mockElement = new Mock<IWebElement>();
            mockElement
                .Setup(c => c.Enabled)
                .Returns(false);

            var mockDriver = new Mock<IWebDriver>();
            mockDriver.Setup
                (f => f.FindElement(It.IsAny<By>()))
                .Returns(mockElement.Object);

            var element = SetupElementHandler(mockDriver.Object);

            Assert.False(element.WaitForEnabled());
        }

        [Fact]
        public void WaitForEnabledThrowsIfTimesOut()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .Setup(f => f.FindElement(It.IsAny<By>()))
                .Throws<NoSuchElementException>();

            var element = SetupElementHandler(mockDriver.Object);

            Assert.Throws<WebDriverTimeoutException>(() => element.WaitForEnabled());
        }

        [Fact]
        public void WaitForDisabledReturnsTrueIfSuccessful()
        {
            var mockElement = new Mock<IWebElement>();
            mockElement
                .Setup(c => c.Enabled)
                .Returns(false);

            var mockDriver = new Mock<IWebDriver>();
            mockDriver.Setup
                (f => f.FindElement(It.IsAny<By>()))
                .Returns(mockElement.Object);

            var element = SetupElementHandler(mockDriver.Object);

            Assert.True(element.WaitForDisabled());
        }

        [Fact]
        public void WaitForDisabledReturnsFalseIfUnsuccessful()
        {
            var mockElement = new Mock<IWebElement>();
            mockElement
                .Setup(c => c.Enabled)
                .Returns(true);

            var mockDriver = new Mock<IWebDriver>();
            mockDriver.Setup
                (f => f.FindElement(It.IsAny<By>()))
                .Returns(mockElement.Object);

            var element = SetupElementHandler(mockDriver.Object);

            Assert.False(element.WaitForDisabled());
        }

        [Fact]
        public void WaitForDisabledThrowsIfTimesOut()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .Setup(f => f.FindElement(It.IsAny<By>()))
                .Throws<NoSuchElementException>();

            var element = SetupElementHandler(mockDriver.Object);

            Assert.Throws<WebDriverTimeoutException>(() => element.WaitForDisabled());
        }

        [Fact]
        public void GetAttributeProperlyReturnsExpectedAttribute()
        {
            var mockElement = new Mock<IWebElement>();
            mockElement
                .Setup(c => c.GetAttribute(It.IsAny<string>()))
                .Returns("Success");

            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .Setup(f => f.FindElement(It.IsAny<By>()))
                .Returns(mockElement.Object);

            var element = SetupElementHandler(mockDriver.Object);

            Assert.Equal("Success", element.GetAttribute("Test"));
        }

        [Fact]
        public void GetAttributeThrowsOnTimeout()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .Setup(f => f.FindElement(It.IsAny<By>()))
                .Throws<NoSuchElementException>();

            var element = SetupElementHandler(mockDriver.Object);

            Assert.Throws<WebDriverTimeoutException>(() => element.GetAttribute("Test"));
        }

        private ElementHandler SetupElementHandler(IWebDriver driver)
        {
            return new ElementHandler(driver)
                .SetLocator(By.Id("test"))
                .SetTimeout(TimeSpan.FromMilliseconds(20))
                .SetPollingIntervalMilliseconds(10);
        }
    }
}
