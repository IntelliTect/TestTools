using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using Xunit;

namespace IntelliTect.TestTools.Selenate.Examples
{
    public class BasicDriverInteractions : IDisposable
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
            test.Url = "http://the-internet.herokuapp.com/";
        }

        [Fact]
        public void Click()
        {
            IWebDriver test = new ChromeDriver();
        }

        public void Dispose()
        {
            _Driver.Dispose();
        }
    }
}
