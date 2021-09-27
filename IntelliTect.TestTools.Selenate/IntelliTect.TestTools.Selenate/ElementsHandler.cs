using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntelliTect.TestTools.Selenate
{
    /// <summary>
    /// Main class for handling interactions with a group of IWebElements.
    /// </summary>
    public class ElementsHandler : HandlerBase
    {
        /// <summary>
        /// Constructor to wrap a specific instace of a WebDriver and to set the locator method when interacting with WebElements
        /// </summary>
        /// <param name="driver">The WebDriver to wrap.</param>
        /// <param name="locator">Method for locating elements.</param>
        public ElementsHandler(IWebDriver driver, By locator) : base(driver)
        {
            Locator = locator;
        }

        public By Locator { get; private set; }

        /// <summary>
        /// Sets the locator to use for operations within this instance.
        /// </summary>
        /// <param name="by">Method to find multiple elements.</param>
        /// <returns>this</returns>
        public ElementsHandler SetLocator(By by)
        {
            Locator = by;
            return this;
        }

        /// <summary>
        /// Sets the maximum time that this instance will retry a specific interaction with a group of WebElements before throwing.
        /// </summary>
        /// <param name="timeout">Duration to retry an action before throwing.</param>
        /// <returns>this</returns>
        public ElementsHandler SetTimeout(TimeSpan timeout)
        {
            return SetTimeout<ElementsHandler>(timeout);
        }

        /// <summary>
        /// Sets the maximum time in seconds that this instance will retry a specific interaction with a group of WebElements before throwing.
        /// </summary>
        /// <param name="timeoutInSeconds">Duration to retry an action before throwing.</param>
        /// <returns>this</returns>
        public ElementsHandler SetTimeoutSeconds(int timeoutInSeconds)
        {
            return SetTimeout<ElementsHandler>(TimeSpan.FromSeconds(timeoutInSeconds));
        }

        /// <summary>
        /// Sets the amount of time this instance will wait in between retrying a specific interaction.
        /// </summary>
        /// <param name="pollingInterval">Time to wait in between retrying an action.</param>
        /// <returns>this</returns>
        public ElementsHandler SetPollingInterval(TimeSpan pollingInterval)
        {
            return SetPollingInterval<ElementsHandler>(pollingInterval);
        }

        /// <summary>
        /// Sets the amount of time in milliseconds this instance will wait in between retrying a specific interaction.
        /// </summary>
        /// <param name="pollIntervalInMilliseconds">Time to wait in between retrying an action.</param>
        /// <returns>this</returns>
        public ElementsHandler SetPollingIntervalMilliseconds(int pollIntervalInMilliseconds)
        {
            return SetPollingInterval<ElementsHandler>(TimeSpan.FromMilliseconds(pollIntervalInMilliseconds));
        }

        /// <summary>
        /// Checks if any element found by <see cref="Locator"/> contains the matching text.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <returns>True if the text is found; false if it is not.</returns>
        public bool ContainsText(string text)
        {
            IWait<IWebDriver> wait = Wait;
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            try
            {
                return wait.Until(d => d.FindElements(Locator).Any(h => h.Text == text));
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if any element found by <see cref="Locator"/> matches a predicate.
        /// </summary>
        /// <param name="predicate">The criteria to attempt to match on.</param>
        /// <returns></returns>
        public IWebElement GetSingleExistingElement(Func<IWebElement, bool> predicate)
        {
            IWait<IWebDriver> wait = Wait;
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            return wait.Until(d =>
            {
                var foundElems = d.FindElements(Locator);
                if (foundElems is null || foundElems.Count == 0) throw new NoSuchElementException($"No element found matching pattern: {Locator}");
                var foundElem = foundElems.Where(predicate).ToList();
                if (foundElem.Count != 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(predicate), 
                        "The search criteria did not return exactly one result.");
                }
                return foundElem[0];
            });
        }
    }
}
