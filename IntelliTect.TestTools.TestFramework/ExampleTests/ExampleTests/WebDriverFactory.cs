using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

namespace ExampleTests
{
#pragma warning disable CA1001 // Types that own disposable fields should be disposable; Driver should be disposed by DI container, not the factory
    public class WebDriverFactory
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        public WebDriverFactory(string browserType)
        {
            _BrowserType = browserType;
            Driver = GetWebDriver;
        }

        public Func<IServiceProvider, IWebDriver> Driver { get; private set; }

        private IWebDriver GetWebDriver(IServiceProvider service)
        {
            if (_BrowserType == "Chrome")
                _Driver = new ChromeDriver(Directory.GetCurrentDirectory());
            else
                _Driver = new ChromeDriver(Directory.GetCurrentDirectory());
            return _Driver;
        }

        private IWebDriver? _Driver;
        private string _BrowserType;
    }
}
