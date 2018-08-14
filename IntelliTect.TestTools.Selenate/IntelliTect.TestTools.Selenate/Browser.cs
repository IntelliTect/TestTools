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
        InternetExplorer
    }
    public class Browser
    {
        public Browser(BrowserType browser)
        {
            InitDriver(browser);
        }

        public IWebDriver Driver { get; set; }

        // Mike C: Find a good way to abstract this out. Different projects will have different requirements here.
        // Good candidate for an extension? Or maybe an abstract class?
        public void InitDriver(BrowserType browser)
        {
            Driver?.Quit();

            switch (browser)
            {
                case BrowserType.Chrome:
                    ChromeOptions chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("--disable-extension");
                    chromeOptions.AddArgument("no-sandbox");
                    chromeOptions.AddArgument("disable-infobars");
                    chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
                    chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
                    Driver = new ChromeDriver(Directory.GetCurrentDirectory(), chromeOptions, TimeSpan.FromMinutes(1));

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
                    Driver = new InternetExplorerDriver(ieCaps);
                    break;
                default:
                    throw new ArgumentException($"Unknown browser: {browser}");
            }

            Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(2);
        }

        public void TakeScreenshot()
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "screenshot", $"{((RemoteWebDriver)this.Driver).Capabilities.BrowserName}_{DateTime.Now}");

            Screenshot screenshot;

            Console.WriteLine($"Saving screenshot to location: {fullPath}");
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "screenshot"));
            try
            {
                screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
                screenshot.SaveAsFile(fullPath, ScreenshotImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Screenshot was not saved due to {ex.Message}");
            }
        }

        /// <summary>
        /// Wraps the Selenium Driver's native web element to wait until the element exists before returning.
        /// If you need to verify an element DOESN'T exist, then call Browser.Driver.FindElement directly.
        /// </summary>
        /// <param name="by">Selenium "By" statement to find the element</param>
        /// <param name="secondsToWait">Seconds to wait while retrying before failing</param>
        /// <returns></returns>
		public WebElement FindElement(By by, int secondsToWait = 5)
        {
            Console.WriteLine($"Attempting to find element using selector: {by}");

            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToWait));
            try
            {
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(by));
            }
            catch (WebDriverTimeoutException)
            {
                return new WebElement(by, Driver);
            }
            

            return new WebElement(Driver.FindElement(by), by, Driver);
        }

        /// <summary>
        /// Wraps the Selenium Driver's native web element to wait until at least one element exists before returning.
        /// If you need to verify an element DOESN'T exist, then call Browser.Driver.FindElements directly.
        /// </summary>
        /// <param name="by">Selenium "By" statement to find the element</param>
        /// <param name="secondsToWait">Seconds to wait while retrying before failing</param>
        /// <returns></returns>
        public ReadOnlyCollection<WebElement> FindElements(By by, int secondsToWait = 5)
        {
            Console.WriteLine($"Attempting to find all elements using selector: {by}");

            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToWait));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(by));

            return new ReadOnlyCollection<WebElement>(
                            Driver.FindElements(by)
                                    .Select(webElement => new WebElement(by, Driver))
                                    .ToList());
        }

        /// <summary>
        ///Waits until a function evaluates to true OR times out after a specified period of time
        /// </summary>
        /// <param name="func">Function to evaluate</param>
        /// <param name="secondsToWait">Secondes to wait until timeout / return false</param>
        /// <returns></returns>
        public bool WaitFor(Func<bool> func, int secondsToWait = 15)
        {
            DateTime end = DateTime.Now.AddSeconds(secondsToWait);
            do
            {
                // Do a brief wait first, on the assumption that C# hasn't evaluated the delegate yet
                // This is to test to see if Selenium is tying up processes on the webpage under test
                Task.Delay(500).Wait();
                try
                {
                    if (func())
                    {
                        return true;
                    }
                }
                catch (WebDriverTimeoutException)
                {
                    return false;
                }

            } while (DateTime.Now <= end);
            return false;
        }

        public bool IsElementDisplayedAndExisting(By by, int secondsToWait = 15)
        {
            // Now that we have a WebElement.Initialized property, see if it makes sense to refactor this logic to look at that instead?
            // Or is it worth still re-trying, just in case the site was in a bad state prior to calling this?
            // If the latter, the default secondsToWait can be kicked down to maybe 5.
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToWait));
            try
            {
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(by));
            }
            catch (WebDriverTimeoutException) { }
            IReadOnlyCollection<IWebElement> elements = Driver.FindElements(by);
            if (elements.Count > 0)
            {
                return elements.First().Displayed;
            }
            return false;
        }

        public void FrameSwitchAttempt(params By[] bys)
        {
            var exceptions = new List<Exception>();
            for (int i = 0; i < 50; i++)
            {
                try
                {
                    Task.Delay(250).Wait();
                    Driver.SwitchTo().DefaultContent();
                    foreach (By by in bys)
                    {
                        // Don't use our WebElement extension for this as it has trouble being casted to IWebElementReference
                        IWebElement element = Driver.FindElement(by);
                        Console.WriteLine("Switching to frame " + by);
                        Driver.SwitchTo().Frame(element);
                    }
                    return;
                }
                catch (NoSuchFrameException e)
                {
                    exceptions.Add(e);
                }
                catch (InvalidOperationException e)
                {
                    exceptions.Add(e);
                }
                catch (StaleElementReferenceException e)
                {
                    exceptions.Add(e);
                }
                catch (NotFoundException e)
                {
                    exceptions.Add(e);
                };
            }
            throw new AggregateException(exceptions);
        }

        public bool SwitchWindow(string title)
        {
            for (int i = 0; i < 50; i++)
            {
                string currentWindow = null;
                try
                {
                    currentWindow = Driver.CurrentWindowHandle;
                }
                catch (NoSuchWindowException)
                {
                    currentWindow = "";
                }

                var availableWindows = new List<string>(Driver.WindowHandles);

                foreach (string w in availableWindows)
                {
                    if (w != currentWindow)
                    {
                        try
                        {
                            Driver.SwitchTo().Window(w);
                            if (Driver.Title == title)
                            {
                                return true;
                            }
                            else
                            {
                                Driver.SwitchTo().Window(currentWindow);
                            }
                        }
                        catch (NoSuchWindowException) { }
                    }
                }

                Task.Delay(500).Wait();
            }
            return false;
        }

        public IAlert Alert(int numberOfRetries = 50)
        {
            for (int i = 0; i < numberOfRetries; i++)
            {
                try
                {
                    return Driver.SwitchTo().Alert();
                }
                catch (NoAlertPresentException) { }
                catch (UnhandledAlertException) { }
                Task.Delay(500).Wait();
            }
            return null;
        }
    }
}
