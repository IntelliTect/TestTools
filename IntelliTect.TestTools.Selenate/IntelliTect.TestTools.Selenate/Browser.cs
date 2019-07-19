using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;

namespace IntelliTect.TestTools.Selenate
{
    /// <summary>
    /// Enum representing all supported browser types of <see cref="Browser"/>
    /// </summary>
    public enum BrowserType
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Chrome,
        HeadlessChrome,
        InternetExplorer,
        Firefox,
        Edge
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// Wrapper around a <see cref="IWebDriver"/> that provides numerous
    /// utilities for interacting with the underlying driver.
    /// </summary>
    public class Browser : IDisposable
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

        /// <summary>
        /// The underlying <see cref="IWebDriver"/> of the <see cref="Browser"/> instance.
        /// </summary>
        public IWebDriver Driver { get; }

        /// <summary>
        /// Wraps the Selenium Driver's native web element to wait until the element exists before returning.
        /// </summary>
        /// <param name="by">Selenium "By" statement to find the element</param>
        /// <param name="secondsToWait">Seconds to wait while retrying before failing</param>
        /// <returns></returns>
        public IWebElement FindElement(By by, int secondsToWait = 15)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToWait));
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
            return wait.Until(f => Driver.FindElement(by));
        }

        /// <summary>
        /// Wraps the Selenium Driver's native web element to wait until at least one element exists before returning.
        /// </summary>
        /// <param name="by">Selenium "By" statement to find the element</param>
        /// <param name="secondsToWait">Seconds to wait while retrying before failing</param>
        /// <returns></returns>
        public IReadOnlyCollection<IWebElement> FindElements(By by, int secondsToWait = 15)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToWait));
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
            try
            {
                return wait.Until(f => Driver.FindElements(by));
            }
            catch (WebDriverTimeoutException)
            {
                return Array.Empty<IWebElement>();
            }
        }

        /// <summary>
        ///Waits until a function evaluates to true OR times out and returns false after a specified period of time
        /// </summary>
        /// <param name="func">Function to evaluate</param>
        /// <param name="secondsToWait">Seconds to wait until timeout / return false</param>
        /// <returns></returns>
        public bool WaitUntil(Func<bool> func, int secondsToWait = 15)
        {
            WebDriverWait wait = new WebDriverWait( Driver, TimeSpan.FromSeconds( secondsToWait ) );
            wait.IgnoreExceptionTypes( 
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException),
                typeof(ElementNotVisibleException),
                typeof(InvalidElementStateException));
            try
            {
                if ( wait.Until( f => func() ) )
                {
                    return true;
                }
                return false;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }

        }

        /// <summary>
        /// Take a screenshot of the browser, and save it to a "screenshots"
        /// directory inside the current working directory.
        /// </summary>
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

        /// <summary>
        /// Attempts to switch to the window by title for a certain number of seconds before failing if the switch is unsuccessful
        /// </summary>
        /// <param name="title"></param>
        /// <param name="secondsToWait"></param>
        /// <returns></returns>
        public void SwitchWindow(string title, int secondsToWait = 15)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToWait));
            wait.IgnoreExceptionTypes(typeof(NoSuchWindowException));
            wait.Until(w => Driver.SwitchTo().Window(title));
        }

        /// <summary>
        /// Checks for a present alert for a certain number of seconds before continuing
        /// </summary>
        /// <param name="secondsToWait"></param>
        /// <returns></returns>
        public IAlert FindAlert(int secondsToWait = 15)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToWait));
            wait.IgnoreExceptionTypes(typeof(NoAlertPresentException), typeof(UnhandledAlertException));
            return wait.Until( a => Driver.SwitchTo().Alert() );
        }

        /// <summary>
        /// Switches to each frame in succession to avoid having to explicitely call SwitchTo() multipled times for nested frames
        /// </summary>
        /// <param name="secondsToWait"></param>
        /// <param name="bys"></param>
        /// <returns></returns>
        public void FrameSwitchAttempt(int secondsToWait = 15, params By[] bys)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToWait));
            wait.IgnoreExceptionTypes(
                typeof(NoSuchFrameException), 
                typeof(InvalidOperationException), 
                typeof(StaleElementReferenceException), 
                typeof(NotFoundException));

            // Note, some applications will break out of switching to a frame if something on page is still loading.
            // See if restarting the whole search like we currently do on PTT is necessary, or if we can just wait for something to finish loading
            foreach (By by in bys)
            {
                IWebElement element = FindElement(by);
                wait.Until(f => Driver.SwitchTo().Frame(element));
            }
        }

        /// <summary>
        /// Disposes the current Selenium Driver
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Has Selenium already been disposed?
        /// </summary>
        private bool _Disposed;

        /// <summary>
        /// Create a new driver for the given browser type.
        /// </summary>
        /// <param name="browser">The browser to create a driver for.</param>
        /// <returns>The driver created.</returns>
        protected IWebDriver InitDriver(BrowserType browser)
        {
            Driver?.Quit();

            IWebDriver driver = null;

            switch (browser)
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
                    EdgeOptions edgeOptions = new EdgeOptions();
                    edgeOptions.UseInPrivateBrowsing = true;
                    edgeOptions.UnhandledPromptBehavior = UnhandledPromptBehavior.Accept;
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
                    throw new ArgumentException($"Unknown browser: {browser}");
            }

            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(2);
            return driver;
        }

        /// <summary>
        /// Disposes the current Selenium Driver
        /// </summary>
        /// <param name="disposing">Did the call come from Dispose()?</param>
        protected virtual void Dispose(bool disposing)
        {
            if(Disposed)
            {
                return;
            }

            if(disposing)
            {
                Driver?.Dispose();
            }

            Disposed = true;
        }
    }
}
