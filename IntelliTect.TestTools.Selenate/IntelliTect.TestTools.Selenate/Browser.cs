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
        /// Initializes a Selenium webdriver with some basic settings that work for many websites
        /// </summary>
        /// <param name="browser"></param>
        public Browser(BrowserType browser)
        {
            Driver = InitDriver(browser);
        }

        /// <summary>
        /// Uses an 
        /// </summary>
        /// <param name="driver"></param>
        public Browser(IWebDriver driver)
        {
            Driver = driver;
        }

        public IWebDriver Driver { get; }

        // Mike C: Find a good way to abstract this out. Different projects will have different requirements here.
        // Good candidate for an extension? Or maybe an abstract class?
        public IWebDriver InitDriver(BrowserType browser)
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

        /// <summary>
        /// Wraps the Selenium Driver's native web element to wait until the element exists before returning.
        /// If you need to verify an element DOESN'T exist, then call Browser.Driver.FindElement directly.
        /// </summary>
        /// <param name="by">Selenium "By" statement to find the element</param>
        /// <param name="secondsToWait">Seconds to wait while retrying before failing</param>
        /// <returns></returns>
		public Task<IWebElement> FindElement(By by, int secondsToWait = 5)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitForSeconds<NoSuchElementException, StaleElementReferenceException, IWebElement>(() => Driver.FindElement(by));
        }

        /// <summary>
        /// Wraps the Selenium Driver's native web element to wait until at least one element exists before returning.
        /// If you need to verify an element DOESN'T exist, then call Browser.Driver.FindElements directly.
        /// </summary>
        /// <param name="by">Selenium "By" statement to find the element</param>
        /// <param name="secondsToWait">Seconds to wait while retrying before failing</param>
        /// <returns></returns>
        public Task<IReadOnlyCollection<IWebElement>> FindElements(By by, int secondsToWait = 5)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitForSeconds<NoSuchElementException, StaleElementReferenceException, IReadOnlyCollection<IWebElement>>(() => Driver.FindElements(by));
        }

        /// <summary>
        ///Waits until a function evaluates to true OR times out after a specified period of time
        /// </summary>
        /// <param name="func">Function to evaluate</param>
        /// <param name="secondsToWait">Secondes to wait until timeout / return false</param>
        /// <returns></returns>
        public Task<bool> WaitFor(Func<bool> func, int secondsToWait = 15)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitForSeconds<
                NoSuchElementException,
                StaleElementReferenceException,
                ElementNotVisibleException,
                InvalidElementStateException,
                bool>(func);
        }

        /// <summary>
        /// Switches to each frame in succession to avoid having to explicitely call SwitchTo() multipled times for nested frames
        /// </summary>
        /// <param name="bys"></param>
        /// <returns></returns>
        public async Task FrameSwitchAttempt(params By[] bys)
        {
            // Note, some applications (i.e. CCB) will break out of switching to a frame if it's still loading.
            // See if restarting the whole search like we currently do on PTT is necessary, or if we can just wait for something to finish loading
            ConditionalWait wait = new ConditionalWait();
            foreach (By by in bys)
            {
                IWebElement element = Driver.FindElement(by);
                await wait.WaitForSeconds<
                            NoSuchFrameException,
                            InvalidOperationException,
                            StaleElementReferenceException,
                            NotFoundException>
                        (() => Driver.SwitchTo().Frame(element));
            }
        }

        public async Task SwitchWindow(string title)
        {
            ConditionalWait wait = new ConditionalWait();
            await wait.WaitForSeconds<NoSuchWindowException>(() => Driver.SwitchTo().Window(title));
        }

        public Task<IAlert> Alert(int numberOfRetries = 50)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitForSeconds<
                NoAlertPresentException,
                UnhandledAlertException,
                IAlert>
                (() => Driver.SwitchTo().Alert());
        }

        public void TakeScreenshot()
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "screenshot", $"{((RemoteWebDriver)this.Driver).Capabilities.BrowserName}_{DateTime.Now:yyyy.MM.dd_hh.mm.ss}");
            Console.WriteLine($"Saving screenshot to location: {fullPath}");
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "screenshot"));
            if (Driver is ITakesScreenshot takeScreenshot)
            {
                Screenshot screenshot = takeScreenshot.GetScreenshot();
                screenshot?.SaveAsFile(fullPath, ScreenshotImageFormat.Png);
            }
        }
    }
}
