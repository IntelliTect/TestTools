using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using Xunit;

namespace IntelliTect.TestTools.Selenate.Examples
{
#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class BasicDriverInteractions : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        public BasicDriverInteractions()
        {
            _Driver = new WebDriverFactory(BrowserType.Chrome).GetDriver();
        }

        private readonly IWebDriver _Driver;

        [Fact]
        public void Navigate()
        {
            DriverHandler driver = new DriverHandler(_Driver);
            driver.NavigateToPage("http://the-internet.herokuapp.com/");
            Assert.Equal("The Internet", driver.WrappedDriver.Title);
        }

        [Fact]
        public void Click()
        {
            DriverHandler driver = new DriverHandler(_Driver);
            driver.NavigateToPage("http://the-internet.herokuapp.com/")
                .FindElement(By.CssSelector("a[href='/abtest']"))
                .ClickWhenReady();
            Assert.Equal("A/B Test Control", driver.FindElement(By.CssSelector("div[class='example']>h3"))
                .GetElementText());
        }

        public void Dispose()
        {
            _Driver.Dispose();
        }
    }
}
