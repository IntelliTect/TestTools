using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

namespace ExampleTests
{
    public class WebDriverFactory
    {
        public WebDriverFactory(string browserType)
        {
            _BrowserType = browserType;
            Driver = GetWebDriver;
        }

        public Func<IServiceProvider, IWebDriver> Driver { get; private set; }

        private IWebDriver GetWebDriver(IServiceProvider service)
        {
            return new ChromeDriver(Directory.GetCurrentDirectory());
        }

        private string _BrowserType;
    }
}
