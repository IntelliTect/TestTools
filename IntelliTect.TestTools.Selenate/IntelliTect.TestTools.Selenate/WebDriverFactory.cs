using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using System;
using System.IO;

namespace IntelliTect.TestTools.Selenate
{
    /// <summary>
    /// Class for instantiating "everyday" WebDrivers.
    /// </summary>
    public class WebDriverFactory
    {
        /// <summary>
        /// The type of browser to drive with a Selenium WebDriver.
        /// </summary>
        /// <param name="browserType">The type of WebDriver to instantiate to drive a specific browser.</param>
        public WebDriverFactory(BrowserType browserType)
        {
            BrowserType = browserType; 
        }

        private BrowserType BrowserType { get; set; }

        /// <summary>
        /// Gets the driver specified in the constructor. 
        /// Do not forget to dispose of your driver after each test or test collection
        /// </summary>
        public IWebDriver GetDriver()
        {
            IWebDriver driver;
            switch (BrowserType)
            {
                case BrowserType.Chrome:
                    ChromeOptions chromeOptions = new();
                    chromeOptions.AddArgument("--disable-extension");
                    chromeOptions.AddArgument("--no-sandbox");
                    chromeOptions.AddArgument("--disable-infobars");
                    chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
                    chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
                    driver = new ChromeDriver(Directory.GetCurrentDirectory(), chromeOptions);
                    break;
                case BrowserType.Firefox:
                    FirefoxOptions ffOptions = new();
                    ffOptions.AddArgument("-safe-mode");
                    driver = new FirefoxDriver(Directory.GetCurrentDirectory(), ffOptions);
                    break;
                case BrowserType.Edge:
                    EdgeOptions edgeOptions = new()
                    {
                        UnhandledPromptBehavior = UnhandledPromptBehavior.Accept
                    };
                    driver = new EdgeDriver(Directory.GetCurrentDirectory(), edgeOptions);
                    break;
                case BrowserType.HeadlessChrome:
                    ChromeOptions headlessChromeOptions = new();
                    headlessChromeOptions.AddArgument("--disable-extension");
                    headlessChromeOptions.AddArgument("--headless");
                    headlessChromeOptions.AddArgument("--no-sandbox");
                    headlessChromeOptions.AddArgument("--disable-infobars");
                    headlessChromeOptions.AddUserProfilePreference("credentials_enable_service", false);
                    headlessChromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
                    driver = new ChromeDriver(Directory.GetCurrentDirectory(), headlessChromeOptions);
                    break;
                default:
                    throw new ArgumentException($"Unknown browser: {BrowserType}");
            }

            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(2);
            return driver;
        }
    }
}
