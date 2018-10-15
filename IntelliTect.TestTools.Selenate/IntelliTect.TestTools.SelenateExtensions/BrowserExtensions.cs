using IntelliTect.TestTools.Selenate;
using OpenQA.Selenium;
using System;
using System.Threading.Tasks;

namespace IntelliTect.TestTools.SelenateExtensions
{
    public static class BrowserExtensions
    {
        /// <summary>
        /// Switches to each frame in succession to avoid having to explicitely call SwitchTo() multipled times for nested frames
        /// </summary>
        /// <param name="bys"></param>
        /// <returns></returns>
        public static async Task FrameSwitchAttempt(this Browser browser, int secondsToWait = 15, params By[] bys)
        {
            // Note, some applications will break out of switching to a frame if something on page is still loading.
            // See if restarting the whole search like we currently do on PTT is necessary, or if we can just wait for something to finish loading
            ConditionalWait wait = new ConditionalWait();
            foreach (By by in bys)
            {
                IWebElement element = browser.Driver.FindElement(by);
                await wait.WaitFor<
                            NoSuchFrameException,
                            InvalidOperationException,
                            StaleElementReferenceException,
                            NotFoundException,
                            IWebDriver>
                        (() => browser.Driver.SwitchTo().Frame(element), TimeSpan.FromSeconds(secondsToWait));
            }
        }
    }
}
