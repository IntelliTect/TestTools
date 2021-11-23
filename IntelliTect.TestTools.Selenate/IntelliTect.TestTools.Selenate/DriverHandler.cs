using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace IntelliTect.TestTools.Selenate
{
    /// <summary>
    /// Class to handle interactions with a Selenium WebDriver
    /// </summary>
    public class DriverHandler : HandlerBase
    {
        /// <summary>
        /// Constructor to wrap a specific instace of a WebDriver
        /// </summary>
        /// <param name="driver">The WebDriver to wrap</param>
        public DriverHandler(IWebDriver driver) : base(driver) {  }

        private FileInfo? ScreenshotLocation { get; set; }

        /// <summary>
        /// Sets the maximum time that this instance will retry a specific interaction with a WebDriver before throwing.
        /// </summary>
        /// <param name="timeout">Duration to retry an action before throwing.</param>
        /// <returns>this</returns>
        public DriverHandler SetTimeout(TimeSpan timeout)
        {
            return SetTimeout<DriverHandler>(timeout);
        }

        /// <summary>
        /// Sets the maximum time in seconds that this instance will retry a specific interaction with a WebDriver before throwing.
        /// </summary>
        /// <param name="timeoutInSeconds">Duration to retry an action before throwing.</param>
        /// <returns>this</returns>
        public DriverHandler SetTimeoutSeconds(int timeoutInSeconds)
        {
            return SetTimeout<DriverHandler>(TimeSpan.FromSeconds(timeoutInSeconds));
        }

        /// <summary>
        /// Sets the amount of time this instance will wait in between retrying a specific interaction.
        /// </summary>
        /// <param name="pollingInterval">Time to wait in between retrying an action.</param>
        /// <returns>this</returns>
        public DriverHandler SetPollingInterval(TimeSpan pollingInterval)
        {
            return SetPollingInterval<DriverHandler>(pollingInterval);
        }

        /// <summary>
        /// Sets the amount of time in milliseconds this instance will wait in between retrying a specific interaction.
        /// </summary>
        /// <param name="pollIntervalInMilliseconds">Time to wait in between retrying an action.</param>
        /// <returns>this</returns>
        public DriverHandler SetPollingIntervalMilliseconds(int pollIntervalInMilliseconds)
        {
            return SetPollingInterval<DriverHandler>(TimeSpan.FromMilliseconds(pollIntervalInMilliseconds));
        }

        /// <summary>
        /// Sets the location that will be used for saving a screenshot.
        /// </summary>
        /// <param name="location">The location to save a screensot of the browser being driven by the current WebDriver</param>
        /// <returns>this</returns>
        public DriverHandler SetScreenshotLocation(FileInfo location)
        {
            ScreenshotLocation = location;
            return this;
        }

        /// <summary>
        /// Send the browser being driven by the current WebDriver to a particular URL
        /// </summary>
        /// <param name="uri">The page to go to by URI</param>
        /// <returns>this</returns>
        public DriverHandler NavigateToPage(Uri uri)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));
            WrappedDriver.Navigate().GoToUrl(uri);
            return this;
        }

        /// <summary>
        /// Send the browser being driven by the current WebDriver to a particular URL
        /// </summary>
        /// <param name="uri">The page to go to by string</param>
        /// <returns>this</returns>
        public DriverHandler NavigateToPage(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri)) throw new ArgumentNullException(nameof(uri));
            return NavigateToPage(new Uri(uri));
        }

        /// <summary>
        /// Create an ElementHandler with the means to interact with a specific element in the browser
        /// </summary>
        /// <param name="by">The method to find an element</param>
        /// <returns>An ElementHandler wrapping interactions with a specific IWebElement</returns>
        public ElementHandler FindElement(By by)
        {
            return new ElementHandler(WrappedDriver, by)
                .SetPollingInterval(PollingInterval)
                .SetTimeout(Timeout);
        }

        /// <summary>
        /// Create an ElementsHandler with the means to interact with a set of elements in the browser
        /// </summary>
        /// <param name="by">The method to find a set of elements</param>
        /// <returns>An ElementsHandler wrapping interactions with a set of IWebElements</returns>
        public ElementsHandler FindElements(By by)
        {
            return new ElementsHandler(WrappedDriver, by)
                .SetPollingInterval(PollingInterval)
                .SetTimeout(Timeout);
        }

        /// <summary>
        /// Gets the currently wrapped Driver's window title
        /// </summary>
        /// <returns>The current window title</returns>
        public string GetWindowTitle()
        {
            return WrappedDriver.Title;
        }

        /// <summary>
        /// Attempts to switch to the window by title for a certain number of seconds before failing if the switch is unsuccessful
        /// </summary>
        /// <param name="title">The title of the window to switch to</param>
        /// <returns>This driver focused on the new window</returns>
        public DriverHandler SwitchToWindow(string title)
        {
            IWait<IWebDriver> wait = Wait;
            wait.IgnoreExceptionTypes(typeof(NoSuchWindowException));
            wait.Until(w => {
                IReadOnlyCollection<string> handles = w.WindowHandles;
                foreach(var h in handles)
                {
                    w.SwitchTo().Window(h);
                    if (w.Title == title) return true;
                }

                // We did not find the correct window.
                return false;
            });
            return this;
        }

        /// <summary>
        /// Checks for a present alert for a certain number of seconds before continuing
        /// </summary>
        /// <returns>The alert found</returns>
        public IAlert SwitchToAlert()
        {
            IWait<IWebDriver> wait = Wait;
            wait.IgnoreExceptionTypes(
                typeof(NoAlertPresentException),
                typeof(UnhandledAlertException));
            return wait.Until(a => a.SwitchTo().Alert());
        }

        /// <summary>
        /// Switches to each frame in succession to avoid having to explicitely call SwitchTo() multipled times for nested frames
        /// </summary>
        /// <param name="bys">The Selenium selectors to find the frame/iframe desired to interact with</param>
        /// <returns>The frame found</returns>
        public DriverHandler SwitchToIFrame(params By[] bys)
        {
            IWait<IWebDriver> wait = Wait;
            wait.IgnoreExceptionTypes(
                typeof(NoSuchFrameException),
                typeof(InvalidOperationException),
                typeof(StaleElementReferenceException),
                typeof(NotFoundException));

            // Note, some applications will break out of switching to a frame if something on page is still loading.
            // See if restarting the whole search like we currently do on PTT is necessary, or if we can just wait for something to finish loading
            foreach (By by in bys)
            {
                wait.Until(f => f.SwitchTo().Frame(f.FindElement(by)));
            }

            return this;
        }

        /// <summary>
        /// Take a screenshot of the browser and save it to the passed in fully qualified path.
        /// Will not throw if the path does not exist.
        /// </summary>
        public void TakeScreenshot()
        {
            if(ScreenshotLocation is null)
            {
                ScreenshotLocation = new FileInfo(
                    Path.Combine(Path.GetTempPath(),
                    "screenshots",
                    $"{((RemoteWebDriver)WrappedDriver).Capabilities.GetCapability("browserName")}_{DateTime.Now:yyyy.MM.dd_hh.mm.ss}.png"));
            }

            Directory.CreateDirectory(ScreenshotLocation.DirectoryName);

            ScreenshotLocation.Delete();

            if (WrappedDriver is ITakesScreenshot takeScreenshot)
            {
                Screenshot screenshot = takeScreenshot.GetScreenshot();
                Debug.WriteLine($"Saving screenshot to location: {ScreenshotLocation.FullName}");
                screenshot?.SaveAsFile(ScreenshotLocation.FullName, ScreenshotImageFormat.Png);
            }
        }
    }
}
