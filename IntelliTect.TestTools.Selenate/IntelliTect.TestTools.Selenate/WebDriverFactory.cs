using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IntelliTect.TestTools.Selenate
{
    /// <summary>
    /// 
    /// </summary>
    public class WebDriverFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="browserType"></param>
        public WebDriverFactory(BrowserType browserType)
        {
            BrowserType = browserType; 
        }

        private BrowserType BrowserType { get; set; }

        /// <summary>
        /// Do not forget to dispose of your driver after each test or test collection
        /// </summary>
        public IWebDriver GetDriver()
        {
            IWebDriver driver;
            switch (BrowserType)
            {
                case BrowserType.Chrome:
                    ChromeOptions chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("--disable-extension");
                    chromeOptions.AddArgument("--no-sandbox");
                    chromeOptions.AddArgument("--disable-infobars");
                    chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
                    chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
                    driver = new ChromeDriver(Directory.GetCurrentDirectory(), chromeOptions);
                    break;
                case BrowserType.InternetExplorer:
                    InternetExplorerOptions ieCaps = new InternetExplorerOptions
                    {
                        EnablePersistentHover = true,
                        EnsureCleanSession = true,
                        EnableNativeEvents = true,
                        IgnoreZoomLevel = true,
                        IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                        RequireWindowFocus = false
                    };
                    driver = new InternetExplorerDriver(Directory.GetCurrentDirectory(), ieCaps);
                    break;
                case BrowserType.Firefox:
                    FirefoxOptions ffOptions = new FirefoxOptions();
                    ffOptions.AddArgument("-safe-mode");
                    driver = new FirefoxDriver(Directory.GetCurrentDirectory(), ffOptions);
                    break;
                case BrowserType.Edge:
                    EdgeOptions edgeOptions = new EdgeOptions
                    {
                        UseInPrivateBrowsing = true,
                        UnhandledPromptBehavior = UnhandledPromptBehavior.Accept
                    };
                    driver = new EdgeDriver(Directory.GetCurrentDirectory(), edgeOptions);
                    break;
                case BrowserType.HeadlessChrome:
                    ChromeOptions headlessChromeOptions = new ChromeOptions();
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
