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
    /// 
    /// </summary>
    public class DriverHandler : HandlerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        public DriverHandler(IWebDriver driver) : base(driver)
        {
            //Driver = driver;
        }

        private FileInfo ScreenshotLocation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public DriverHandler SetTimeout(TimeSpan timeout)
        {
            return SetTimeout<DriverHandler>(timeout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        public DriverHandler SetTimeoutSeconds(int timeoutInSeconds)
        {
            return SetTimeout<DriverHandler>(TimeSpan.FromSeconds(timeoutInSeconds));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pollingInterval"></param>
        /// <returns></returns>
        public DriverHandler SetPollingInterval(TimeSpan pollingInterval)
        {
            return SetPollingInterval<DriverHandler>(pollingInterval);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pollIntervalInMilliseconds"></param>
        /// <returns></returns>
        public DriverHandler SetPollingIntervalMilliseconds(int pollIntervalInMilliseconds)
        {
            return SetPollingInterval<DriverHandler>(TimeSpan.FromMilliseconds(pollIntervalInMilliseconds));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public DriverHandler SetScreenshotLocation(FileInfo location)
        {
            ScreenshotLocation = location;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public DriverHandler NavigateToPage(Uri uri)
        {
            WrappedDriver.Navigate().GoToUrl(uri);
            // Should we check for some thing to be present here?
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public IWebElement FindElement(By by)
        {
            IWait<IWebDriver> wait = Wait;
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            return wait.Until(w => w.FindElement(by));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public IList<IWebElement> FindElements(By by)
        {
            IWait<IWebDriver> wait = Wait;
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            return wait.Until(w => w.FindElements(by));
        }

        /// <summary>
        /// Attempts to switch to the window by title for a certain number of seconds before failing if the switch is unsuccessful
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public DriverHandler SwitchToWindow(string title)
        {
            IWait<IWebDriver> wait = Wait;
            wait.IgnoreExceptionTypes(typeof(NoSuchWindowException));
            wait.Until(w => w.SwitchTo().Window(title));
            return this;
        }

        /// <summary>
        /// Checks for a present alert for a certain number of seconds before continuing
        /// </summary>
        /// <returns></returns>
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
        /// <param name="bys"></param>
        /// <returns></returns>
        public void SwitchToIFrame(params By[] bys)
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
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public void TakeScreenshot()
        //{
        //    FileInfo file = new FileInfo(
        //        Path.Combine(Path.GetTempPath(),
        //        "screenshots",
        //        $"{((RemoteWebDriver)Driver).Capabilities.GetCapability("browserName")}_{DateTime.Now:yyyy.MM.dd_hh.mm.ss}.png"));
        //    Directory.CreateDirectory(file.DirectoryName);
        //    //TakeScreenshot(file);
        //}

        /// <summary>
        /// Take a screenshot of the browser and save it to the passed in fully qualified path.
        /// Will not throw if the path does not exist.
        /// </summary>
        public void TakeScreenshot()
        {
            //if (file == null)
            //{
            //    Debug.WriteLine($"Skipping TakeScreenshot. Argument {nameof(file)} was null.");
            //    return;
            //}

            if(ScreenshotLocation == null)
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
