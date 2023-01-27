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
            IWebDriver? driver;
            if (_BrowserType is "Chrome")
                driver = new ChromeDriver(Directory.GetCurrentDirectory());
            // Add other browser here.
            else
                driver = new ChromeDriver(Directory.GetCurrentDirectory());
            
            if(driver is null) throw new NullReferenceException($"Unable to find driver for {_BrowserType}");
            return driver;
        }

        private string _BrowserType;
    }
}
