using Moq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
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
            mockDriver
                .SetupGet(w => w.WindowHandles)
                .Returns(() => throw new NoSuchWindowException());

            DriverHandler handler = new DriverHandler(mockDriver.Object);
            handler.SetTimeout(TimeSpan.FromMilliseconds(1));

            // Not sure which approach to take here... 
            // Reflection, timing, or neither and handle this in an integrated test?
            PropertyInfo prop = handler.GetType().GetProperty("Timeout", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.Equal(TimeSpan.FromMilliseconds(1), prop.GetValue(handler));

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
            Assert.True(sw.Elapsed < TimeSpan.FromMilliseconds(10), "Timeout did not get set to less than the default value.");
        }

        [Fact]
        public void SetTimeoutSecondsChangesDefaultValue()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .SetupGet(w => w.WindowHandles)
                .Returns(() => throw new NoSuchWindowException());

            DriverHandler handler = new DriverHandler(mockDriver.Object);
            handler.SetTimeoutSeconds(1);
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
            Assert.True(sw.Elapsed < TimeSpan.FromSeconds(5), 
                "Timeout did not get set to more than the default value.");
        }

        [Fact]
        public void SetPollingIntervalChangesDefaultValue()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .SetupGet(w => w.WindowHandles)
                .Returns(() => throw new NoSuchWindowException())
                .Verifiable();

            

            DriverHandler handler = new DriverHandler(mockDriver.Object);
            handler.SetTimeout(TimeSpan.FromSeconds(1));
            handler.SetPollingInterval(TimeSpan.FromMilliseconds(1));
            try
            {
                handler.SwitchToWindow("failedTest");
            }
            catch (WebDriverTimeoutException)
            {
            }

            mockDriver.Verify(w => w.WindowHandles, Times.AtLeast(50));
        }

        [Fact]
        public void SetPollingMillisecondsIntervalChangesDefaultValue()
        {
            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .SetupGet(w => w.WindowHandles)
                .Returns(() => throw new NoSuchWindowException())
                .Verifiable();

            DriverHandler handler = new DriverHandler(mockDriver.Object);
            handler.SetTimeout(TimeSpan.FromSeconds(1));
            handler.SetPollingIntervalMilliseconds(1);
            try
            {
                handler.SwitchToWindow("failedTest");
            }
            catch (WebDriverTimeoutException)
            {
            }

            mockDriver.Verify(w => w.WindowHandles, Times.AtLeast(50));
        }

        // This should be an integrated test
        //[Fact]
        //public void SetScreenshotLocation()
        //{
        //    var mockDriver = new Mock<ITakesScreenshot>();
        //    mockDriver
        //        .Setup(w => w.GetScreenshot()).Returns(() => throw new Exception(""))
        //        .Returns(() => throw new NoSuchWindowException())
        //        .Verifiable();

        //    DriverHandler handler = new DriverHandler(mockDriver.Object);
        //}

        [Fact]
        public void NavigateToPageProperlySetsWebDriverUrl()
        {
            string uri = "http://www.someSuccess.com";
            var mockDriver = new Mock<IWebDriver>();
            mockDriver.SetupProperty(x => x.Url);

            DriverHandler handler = new DriverHandler(mockDriver.Object);
            handler.NavigateToPage(new Uri(uri));
            Assert.Equal(uri, handler.WrappedDriver.Url);
        }

        [Fact]
        public void NavigateToPageWithStringProperlySetsWebDriverUrl()
        {
            string uri = "http://www.someSuccess.com";
            var mockDriver = new Mock<IWebDriver>();
            mockDriver.SetupProperty(x => x.Url);

            DriverHandler handler = new DriverHandler(mockDriver.Object);
            handler.NavigateToPage(uri);
            Assert.Equal(uri, handler.WrappedDriver.Url);
        }

        [Fact]
        public void FindElementReturnsElementHandler()
        {
            var mockElement = new Mock<IWebElement>();
            var mockDriver = new Mock<IWebDriver>();
            mockDriver.Setup(x => x.FindElement(It.IsAny<By>())).Returns(mockElement.Object);

            DriverHandler handler = new DriverHandler(mockDriver.Object);
            ElementHandler elem = handler.FindElement(By.Id("Testing"));
            Assert.NotNull(elem);
        }

        [Fact]
        public void FindElementsReturnsElementsHandler()
        {
            var mockElement = new Mock<IWebElement>();
            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .Setup(x => x.FindElements(It.IsAny<By>()))
                .Returns(new ReadOnlyCollection<IWebElement>(
                    new List<IWebElement> { mockElement.Object }));

            DriverHandler handler = new DriverHandler(mockDriver.Object);
            ElementsHandler elem = handler.FindElements(By.Id("Testing"));
            Assert.NotNull(elem);
        }

        [Fact]
        public void GetWindowTitleReturnsDriverProperty()
        {
            string testTitle = "Test Window Title";
            var mockDriver = new Mock<IWebDriver>();
            mockDriver
                .SetupGet(w => w.Title)
                .Returns(testTitle);

            DriverHandler handler = new DriverHandler(mockDriver.Object);

            string title = handler.GetWindowTitle();

            Assert.Equal(testTitle, title);
        }

        // These all are probably easier to do as integration tests
        //[Fact]
        //public void SwitchWindowInvokesSwitchToWindow()
        //{
        //    var mockNavigation = new Mock<ITargetLocator>();
        //    mockNavigation
        //        .Setup(n => n.Window(It.IsAny<string>()));

        //    var mockDriver = new Mock<IWebDriver>();
        //    mockDriver
        //        .Setup(w => w.SwitchTo())
        //        .Returns(mockNavigation.Object);
        //}

        //[Fact]
        //public void SwitchAlertInvokesSwitchToAlert()
        //{

        //}

        //[Fact]
        //public void SwitchFrameInvokesSwitchToFrame()
        //{

        //}

        //[Fact]
        //public void TakeScreenshotTakesScreenshotWithExpectedLocation()
        //{

        //}

        // Null checks here
        [Fact]
        public void NullConstructorThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DriverHandler(null));
        }
    }
}
