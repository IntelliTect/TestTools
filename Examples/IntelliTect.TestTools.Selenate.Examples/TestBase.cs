using OpenQA.Selenium;
using System;

namespace IntelliTect.TestTools.Selenate.Examples
{
    public class TestBase : IDisposable
    {
        public TestBase()
        {
            _WebDriver = new WebDriverFactory(BrowserType.Chrome).GetDriver();
            _DriverHandler = new DriverHandler(_WebDriver);
        }

        protected readonly IWebDriver _WebDriver;
        protected readonly DriverHandler _DriverHandler;

        public void Dispose()
        {
            _WebDriver.Dispose();
        }
    }
}
