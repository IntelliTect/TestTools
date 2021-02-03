using Moq;
using OpenQA.Selenium;
using System;
using Xunit;

namespace IntelliTect.TestTools.Selenate.Tests
{
    public class ElementHandlerTests
    {
        // SetPollingIntervalMilliseconds
        // SendText
        // ReplaceText
        // GetElementText
        // WaitForVisibleState
        // WaitForInvisibleState
        // WaitForEnabledState
        // WaitForDisabledState

        [Fact]
        public void ClickDoesNotThrowIfSuccessful()
        {
            var mockElement = new Mock<IWebElement>();
            mockElement.Setup(c => c.Click())
                .Verifiable();
            var mockDriver = new Mock<IWebDriver>();
            mockDriver.Setup
                (f => f.FindElement(It.IsAny<By>()))
                .Returns(mockElement.Object);

            var element = SetupElementHandler(mockDriver.Object);
            element.Click();

            mockElement.Verify();
        }

        [Fact]
        public void ClickThrowsIfUnsuccessful()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver.Setup
                (f => f.FindElement(It.IsAny<By>()))
                .Throws<NoSuchElementException>();

            var element = SetupElementHandler(mockDriver.Object);

            Assert.Throws<WebDriverTimeoutException>(() => element.Click());
        }

        [Fact]
        public void SendTextDoesNotThrowIfSuccessful()
        {

        }

        [Fact]
        public void SendTextThrowsIfUnsuccessful()
        {

        }

        [Fact]
        public void SendTextWorksWithKeys()
        {
            var mockElement = new Mock<IWebElement>();
            mockElement.Setup(c => c.SendKeys(It.IsAny<string>()))
                .Verifiable();
            var mockDriver = new Mock<IWebDriver>();
            mockDriver.Setup
                (f => f.FindElement(It.IsAny<By>()))
                .Returns(mockElement.Object);

            var element = SetupElementHandler(mockDriver.Object);
            element.SendKeys(Keys.Enter);

            mockElement.Verify();
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
