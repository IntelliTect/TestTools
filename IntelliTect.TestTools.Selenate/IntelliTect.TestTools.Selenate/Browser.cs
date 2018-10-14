using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public Task<IWebElement> FindElement(By by, int secondsToWait = 15)
        {
            // Figure out a good way to allow a global override on the wait timeout.
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitFor<NoSuchElementException, StaleElementReferenceException, IWebElement>(() => Driver.FindElement(by), TimeSpan.FromSeconds(secondsToWait));
        }

        /// <summary>
        /// Wraps the Selenium Driver's native web element to wait until at least one element exists before returning.
        /// </summary>
        /// <param name="by">Selenium "By" statement to find the element</param>
        /// <param name="secondsToWait">Seconds to wait while retrying before failing</param>
        /// <returns></returns>
        public Task<IReadOnlyCollection<IWebElement>> FindElements(By by, int secondsToWait = 15)
        {
            // NOTE: Per conversation with Yuriy on Thursday, this should return an empty collection if nothing is found.
            // Often times the use case is to assert on the collection, even if nothing is there.
            ConditionalWait wait = new ConditionalWait();
            try
            {
                return wait.WaitFor<NoSuchElementException, StaleElementReferenceException, IReadOnlyCollection<IWebElement>>(() => Driver.FindElements(by), TimeSpan.FromSeconds(secondsToWait));
            }
            catch(AggregateException)
            {
                return null;
            }
        }

        /// <summary>
        ///Waits until a function evaluates to true OR times out after a specified period of time
        /// </summary>
        /// <param name="func">Function to evaluate</param>
        /// <param name="secondsToWait">Secondes to wait until timeout / return false</param>
        /// <returns></returns>
        public async Task<bool> WaitUntil(Func<bool> func, int secondsToWait = 15)
        {
            ConditionalWait wait = new ConditionalWait();
            try
            {
                if (await wait.WaitFor<
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

        /// <summary>
        /// Switches to each frame in succession to avoid having to explicitely call SwitchTo() multipled times for nested frames
        /// </summary>
        /// <param name="bys"></param>
        /// <returns></returns>
        public async Task FrameSwitchAttempt(int secondsToWait = 15, params By[] bys)
        {
            // Note, some applications will break out of switching to a frame if something on page is still loading.
            // See if restarting the whole search like we currently do on PTT is necessary, or if we can just wait for something to finish loading
            ConditionalWait wait = new ConditionalWait();
            foreach (By by in bys)
            {
                IWebElement element = Driver.FindElement(by);
                await wait.WaitFor<
                            NoSuchFrameException,
                            InvalidOperationException,
                            StaleElementReferenceException,
                            NotFoundException,
                            IWebDriver>
                        (() => Driver.SwitchTo().Frame(element), TimeSpan.FromSeconds(secondsToWait));
            }
        }

        /// <summary>
        /// Attempts to switch to the window by title for a certain number of seconds before failing if the switch is unsuccessful
        /// </summary>
        /// <param name="title"></param>
        /// <param name="secondsToWait"></param>
        /// <returns></returns>
        public async Task SwitchWindow(string title, int secondsToWait = 15)
        {
            ConditionalWait wait = new ConditionalWait();
            await wait.WaitFor<NoSuchWindowException>(() => Driver.SwitchTo().Window(title), TimeSpan.FromSeconds(secondsToWait));
        }

        /// <summary>
        /// Checks for a present alert for a certain number of seconds before continuing
        /// </summary>
        /// <param name="secondsToWait"></param>
        /// <returns></returns>
        public Task<IAlert> Alert(int secondsToWait = 15)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitFor<
                NoAlertPresentException,
                UnhandledAlertException,
                IAlert>
                (() => Driver.SwitchTo().Alert(), TimeSpan.FromSeconds(secondsToWait));
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
