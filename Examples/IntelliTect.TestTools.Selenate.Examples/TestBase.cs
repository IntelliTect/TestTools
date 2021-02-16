using OpenQA.Selenium;
using System;

namespace IntelliTect.TestTools.Selenate.Examples
{
    public class TestBase : IDisposable
    {
        public TestBase()
        {
            WebDriver = new WebDriverFactory(BrowserType.Chrome).GetDriver();
            DriverHandler = new DriverHandler(WebDriver);
        }

        protected IWebDriver WebDriver { get; }
        protected DriverHandler DriverHandler { get; }

        public void Dispose()
        {
            WebDriver.Dispose();
        }
    }
}
