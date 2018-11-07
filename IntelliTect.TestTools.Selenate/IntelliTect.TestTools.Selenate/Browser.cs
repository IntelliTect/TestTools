using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace IntelliTect.TestTools.Selenate
{
    public enum BrowserType
    {
        Chrome,
        InternetExplorer,
        Firefox,
        Edge
        // What else is worth supporting? If we support IE, there might be a few others worth supporting
    }
    public class Browser
    {
        /// <summary>
        /// Initializes a Selenium webdriver in Chrome with some basic settings that work for many websites
        /// </summary>
        public Browser()
        {
            Driver = InitDriver(BrowserType.Chrome);
        }

        /// <summary>
        /// Initializes a Selenium webdriver of the specified browser type with some basic settings that work for many websites
        /// </summary>
        /// <param name="browser">The type of browser to instantiate when initializing Selenium Webdriver</param>
        public Browser(BrowserType browser)
        {
            Driver = InitDriver(browser);
        }

        /// <summary>
        /// Uses an existing driver to facilitate applications that need specific driver capabilities not specified in InitDriver
        /// </summary>
        /// <param name="driver">An already initialized Selenium webdriver</param>
        public Browser(IWebDriver driver)
        {
            Driver = driver;
        }

        // Might be worth making this protected
        public IWebDriver Driver { get; }

        /// <summary>
        /// Wraps the Selenium Driver's native web element to wait until the element exists before returning.
        /// </summary>
        /// <param name="by">Selenium "By" statement to find the element</param>
        /// <param name="secondsToWait">Seconds to wait while retrying before failing</param>
        /// <returns></returns>
        public async Task<IWebElement> FindElement(By by, int secondsToWait = 15)
        {
            // Figure out a good way to allow a global override on the wait timeout.
            return await Wait.Until<NoSuchElementException, StaleElementReferenceException, IWebElement>(() => Driver.FindElement(by), TimeSpan.FromSeconds(secondsToWait));
        }

        /// <summary>
        /// Wraps the Selenium Driver's native web element to wait until at least one element exists before returning.
        /// </summary>
        /// <param name="by">Selenium "By" statement to find the element</param>
        /// <param name="secondsToWait">Seconds to wait while retrying before failing</param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<IWebElement>> FindElements(By by, int secondsToWait = 15)
        {
            try
            {
                return await Wait.Until<NoSuchElementException, StaleElementReferenceException, IReadOnlyCollection<IWebElement>>(() => Driver.FindElements(by), TimeSpan.FromSeconds(secondsToWait));
            }
            catch(AggregateException)
            {
                return Array.Empty<IWebElement>();
            }
        }

        /// <summary>
        ///Waits until a function evaluates to true OR times out after a specified period of time
        /// </summary>
        /// <param name="func">Function to evaluate</param>
        /// <param name="secondsToWait">Seconds to wait until timeout / return false</param>
        /// <returns></returns>
        public async Task<bool> WaitUntil(Func<bool> func, int secondsToWait = 15)
        {
            try
            {
                if (await Wait.Until<
                NoSuchElementException,
                StaleElementReferenceException,
                ElementNotVisibleException,
                InvalidElementStateException,
                bool>(func, TimeSpan.FromSeconds(secondsToWait)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (AggregateException) // Worth checking for specific inner exceptions?
            {
                return false;
            }

        }

        public void TakeScreenshot()
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "screenshot", $"{((RemoteWebDriver)this.Driver).Capabilities.BrowserName}_{DateTime.Now:yyyy.MM.dd_hh.mm.ss}.png");
            Console.WriteLine($"Saving screenshot to location: {fullPath}");
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "screenshot"));
            if (Driver is ITakesScreenshot takeScreenshot)
            {
                Screenshot screenshot = takeScreenshot.GetScreenshot();
                screenshot?.SaveAsFile(fullPath, ScreenshotImageFormat.Png);
            }
        }

        protected IWebDriver InitDriver(BrowserType browser)
        {
            Driver?.Quit();

            IWebDriver driver = null;

            switch (browser)
            {
                case BrowserType.Chrome:
                    ChromeOptions chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("--disable-extension");
                    chromeOptions.AddArgument("no-sandbox");
                    chromeOptions.AddArgument("disable-infobars");
                    chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
                    chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
                    driver = new ChromeDriver(Directory.GetCurrentDirectory(), chromeOptions, TimeSpan.FromMinutes(1));
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
                    driver = new InternetExplorerDriver(ieCaps);
                    break;
                case BrowserType.Firefox:
                    throw new NotImplementedException();
                case BrowserType.Edge:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException($"Unknown browser: {browser}");
            }

            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(2);
            return driver;
        }
    }
}
