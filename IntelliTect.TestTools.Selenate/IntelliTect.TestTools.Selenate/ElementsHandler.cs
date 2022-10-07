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
    public class ElementsHandler : ElementBase
    {
        /// <summary>
        /// Constructor to wrap a specific instace of a WebDriver and to set the locator method when interacting with WebElements
        /// </summary>
        /// <param name="driver">The WebDriver to wrap.</param>
        /// <param name="locator">Method for locating elements.</param>
        public ElementsHandler(IWebDriver driver, By locator) : base(driver, locator)
        {
        }


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
        /// Sets the search context for this element (Driver, element, shadow dom, etc.)
        /// </summary>
        /// <param name="searchContext">The context to use for all future searches.</param>
        /// <returns></returns>
        public ElementsHandler SetSearchContext(ISearchContext searchContext)
        {
            SearchContext = searchContext;
            return this;
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
                return wait.Until(_ =>
                {
                    IReadOnlyCollection<IWebElement> elems = SearchContext.FindElements(Locator);
                    return elems.Any(h => h.Text == text);
                });
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
        public IWebElement GetSingleWebElement(Func<IWebElement, bool> predicate)
        {
            IList<IWebElement> elems = GetElements(predicate);

            if (elems.Count is not 1)
            {
                throw new ArgumentOutOfRangeException(nameof(predicate), "The provided predicate did not match exactly one result.");
            }

            return elems[0];
        }

        /// <summary>
        /// Gets all elements found by <see cref="Locator"/>, matching a given predicate.
        /// </summary>
        /// <param name="predicate">The function used to filter to one or more IWebElements</param>
        /// <returns>A list of found IWebElements</returns>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IList<IWebElement> GetAllWebElements(Func<IWebElement, bool> predicate)
        {
            return GetElements(predicate);

        }

        private IList<IWebElement> GetElements(Func<IWebElement, bool> predicate)
        {
            IWait<IWebDriver> wait = Wait;
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));

            return wait.Until(_ =>
            {
                IList<IWebElement> foundElems = SearchContext.FindElements(Locator).Where(predicate).ToList();
                if(foundElems.Any())
                { 
                    return foundElems;
                }
                else
                {
                    // Selenium treats this as a failure and will retry this action until:
                    //  1. Something is returned
                    //  2. The timeout is met and a WebDriverTimeoutException is thrown.
                    return null!;
                }
            });
        }
    }
}
