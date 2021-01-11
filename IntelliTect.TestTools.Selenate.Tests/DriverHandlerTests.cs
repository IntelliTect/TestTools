using Moq;
using OpenQA.Selenium;
using System;
using System.Diagnostics;
using Xunit;

namespace IntelliTect.TestTools.Selenate.Tests
{
    public class DriverHandlerTests
    {
        // Change these to just make sure the property is overridden. Otherwise will take too long to run these.
        [Fact]
        public void SetTimeoutChangesDefaultValue()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver.SetupGet(w => w.WindowHandles).Returns(() => throw new NoSuchWindowException());

            DriverHandler handler = new DriverHandler(mockDriver.Object);
            handler.SetTimeout(TimeSpan.FromMilliseconds(10));
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                handler.SwitchToWindow("failedTest");
            }
            catch(WebDriverTimeoutException)
            { 
            }
            sw.Stop();
            Assert.True(sw.Elapsed < TimeSpan.FromSeconds(5), "Timeout did not get set to less than the default value.");
        }

        [Fact]
        public void SetTimeoutSecondsChangesDefaultValue()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver.SetupGet(w => w.WindowHandles).Returns(() => throw new NoSuchWindowException());

            DriverHandler handler = new DriverHandler(mockDriver.Object);
            handler.SetTimeoutSeconds(10);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                handler.SwitchToWindow("failedTest");
            }
            catch (WebDriverTimeoutException)
            {
            }
            sw.Stop();
            Assert.True(sw.Elapsed > TimeSpan.FromSeconds(5), 
                "Timeout did not get set to more than the default value.");
        }

        [Fact]
        public void SetPollingIntervalChangesDefaultValue()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver.SetupGet(w => w.WindowHandles).Returns(() => throw new NoSuchWindowException());

            DriverHandler handler = new DriverHandler(mockDriver.Object);
            handler.SetTimeout(TimeSpan.FromSeconds(1));
            handler.SetPollingInterval(TimeSpan.FromMilliseconds(10));
            try
            {
                handler.SwitchToWindow("failedTest");
            }
            catch (WebDriverTimeoutException)
            {
            }

        }

        [Fact]
        public void SetPollingMillisecondsIntervalChangesDefaultValue()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver.SetupGet(w => w.WindowHandles).Returns(() => throw new NoSuchWindowException());

            DriverHandler handler = new DriverHandler(mockDriver.Object);
            handler.SetTimeout(TimeSpan.FromSeconds(1));
            handler.SetPollingIntervalMilliseconds(10);
            try
            {
                handler.SwitchToWindow("failedTest");
            }
            catch (WebDriverTimeoutException)
            {
            }

        }

        [Fact]
        public void SetScreenshotLocation()
        {

        }

        [Fact]
        public void NavigateToPageProperlySetsWebDriverUrl()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver.SetupProperty(x => x.Url);

            DriverHandler handler = new DriverHandler(mockDriver.Object);
            handler.NavigateToPage(new Uri("http://www.someSuccess.com"));
            Assert.Equal("http://www.somesuccess.com/", handler.WrappedDriver.Url);
        }

        [Fact]
        public void NavigateToPageWithStringProperlySetsWebDriverUrl()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver.SetupProperty(x => x.Url);

            DriverHandler handler = new DriverHandler(mockDriver.Object);
            handler.NavigateToPage("http://www.someSuccess.com");
            Assert.Equal("http://www.somesuccess.com/", handler.WrappedDriver.Url);
        }

        [Fact]
        public void FindElementReturnsElementHandler()
        {

        }

        [Fact]
        public void FindElementsReturnsElementsHandler()
        {

        }

        [Fact]
        public void GetWindowTitleReturnsDriverProperty()
        {

        }

        [Fact]
        public void SwitchWindowInvokesSwitchToWindow()
        {

        }

        [Fact]
        public void SwitchAlertInvokesSwitchToAlert()
        {

        }

        [Fact]
        public void SwitchFrameInvokesSwitchToFrame()
        {

        }

        [Fact]
        public void TakeScreenshotTakesScreenshotWithExpectedLocation()
        {

        }
    }
}
