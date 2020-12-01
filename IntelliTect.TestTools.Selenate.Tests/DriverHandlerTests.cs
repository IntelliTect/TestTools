using Moq;
using OpenQA.Selenium;
using System;
using Xunit;

namespace IntelliTect.TestTools.Selenate.Tests
{
    public class DriverHandlerTests
    {
        [Fact]
        public void Test()
        {
			var mockDriver = new Mock<IWebDriver>();
            mockDriver.SetupProperty(x => x.Url);

            DriverHandler handler = new DriverHandler(mockDriver.Object);
            handler.NavigateToPage(new Uri("http://www.someSuccess.com"));
            Assert.Equal("http://www.somesuccess.com/", handler.WrappedDriver.Url);
		}
    }
}
