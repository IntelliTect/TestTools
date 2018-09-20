using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTect.TestTools.Selenate
{
    public class WebElement : IWebElement
    {
        public WebElement(IWebElement wrappedElement, By by, IWebDriver driver)
        {
            WrappedElement = wrappedElement;
            By = by;
            _Driver = driver;
        }

        public By By { get; }
        public bool Initialized => WrappedElement != null;

        // Probably swap out the Task<> properties in favor of Get methods for consistency.
        // Or scrap this class entirely (if possible) once retry logic is abstracted out into a single callable method
        string IWebElement.TagName => TagName.ConfigureAwait(false).GetAwaiter().GetResult();
        public Task<string> TagName => RetryAction("tagname");

        string IWebElement.Text => Text.GetAwaiter().GetResult();
        public Task<string> Text => RetryAction("text");

        bool IWebElement.Enabled => Convert.ToBoolean(Enabled.ConfigureAwait(false).GetAwaiter().GetResult());
        public Task<string> Enabled => RetryAction("enabled");

        bool IWebElement.Displayed => Convert.ToBoolean(Displayed.ConfigureAwait(false).GetAwaiter().GetResult());
        public Task<string> Displayed => RetryAction("displayed");

        bool IWebElement.Selected => Convert.ToBoolean(Selected.ConfigureAwait(false).GetAwaiter().GetResult());
        public Task<string> Selected => RetryAction("selected");

        Point IWebElement.Location => new Point();
        public Task<string> Location => RetryAction("location");

        Size IWebElement.Size => new Size();
        public Task<string> Size => RetryAction("size");

        public WebElement FindElement(By by)
        {
            if (!Initialized)
            {
                WrappedElement = _Driver.FindElement(by);
            }

            // Eventually swap this out for our own wait
            WebDriverWait wait = new WebDriverWait(_Driver, TimeSpan.FromSeconds(5));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(by));

            return new WebElement(WrappedElement.FindElement(by), by, _Driver);
        }

        public ReadOnlyCollection<WebElement> FindElements(By by)
        {
            if (!Initialized)
            {
                WrappedElement = _Driver.FindElement(by);
            }

            // Eventually swap this out for our own wait
            WebDriverWait wait = new WebDriverWait(_Driver, TimeSpan.FromSeconds(5));
            try
            {
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(by));
            }
            catch (WebDriverTimeoutException)
            {
                return new ReadOnlyCollection<WebElement>(new List<WebElement>());
            }

            return
                new ReadOnlyCollection<WebElement>(
                    WrappedElement.FindElements(by)
                        .Select(webElement =>
                        new WebElement(WrappedElement.FindElement(by), by, _Driver))
                        .ToList());
        }

        void IWebElement.Clear()
        {
            Clear().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        public Task Clear()
        {
            return RetryAction("clear");
        }

        void IWebElement.SendKeys(string text)
        {
            SendKeys(text).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task SendKeys(string text)
        {
            return RetryAction("sendkeys", text);
        }

        void IWebElement.Submit()
        {
            Submit().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task Submit()
        {
            return RetryAction("submit");
        }

        void IWebElement.Click()
        {
            Click().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task Click()
        {
            return RetryAction("click");
        }

        // Can this easily be abstracted out to an extension method?
        // Seems to make more sense as an extension
        public Task Click(int secondsToRetry)
        {
            return RetryAction( "click", secondsToRetry: secondsToRetry );
        }

        string IWebElement.GetAttribute(string attributeName)
        {
            return GetAttribute(attributeName).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task<string> GetAttribute(string attributeName)
        {
            return RetryAction("getattribute", attributeName);
        }

        string IWebElement.GetProperty(string propertyName)
        {
            return GetProperty(propertyName).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task<string> GetProperty(string propertyName)
        {
            return RetryAction("getproperty", propertyName);
        }

        string IWebElement.GetCssValue(string propertyName)
        {
            return GetCssValue(propertyName).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task<string> GetCssValue(string propertyName)
        {
            return RetryAction("getcss", propertyName);
        }

        private IWebElement WrappedElement { get; set; }
        private readonly IWebDriver _Driver;

        private async Task<string> RetryAction(string action, string text = null, int secondsToRetry = 30)
        {
            DateTime end = DateTime.Now.AddSeconds(secondsToRetry);
            bool reFindElement = !Initialized;
            List<Exception> retryExceptions = new List<Exception>();
            while (DateTime.Now <= end)
            {
                await Task.Delay(250);
                try
                {
                    if (reFindElement)
                    {
                        WrappedElement = _Driver.FindElement(By);
                    }
                    string result = null;
                    switch (action)
                    {
                        case "click":
                            WrappedElement.Click();
                            return null;
                        case "submit":
                            WrappedElement.Submit();
                            return null;
                        case "sendkeys":
                            WrappedElement.SendKeys(text);
                            return null;
                        case "getattribute":
                            result = WrappedElement.GetAttribute(text);
                            return result;
                        case "clear":
                            WrappedElement.Clear();
                            return null;
                        case "getcss":
                            result = WrappedElement.GetCssValue(text);
                            return result;
                        case "getproperty":
                            return WrappedElement.GetProperty(text);
                        case "text":
                            result = WrappedElement.Text;
                            return result;
                        case "enabled":
                            result = WrappedElement.Enabled.ToString();
                            return result;
                        case "displayed":
                            result = WrappedElement.Displayed.ToString();
                            return result;
                        case "selected":
                            result = WrappedElement.Selected.ToString();
                            return result;
                        case "tagname":
                            return WrappedElement.TagName;
                        case "location":
                            return WrappedElement.Location.ToString();
                        case "size":
                            return WrappedElement.Size.ToString();
                        default:
                            throw new InvalidOperationException(
                                "Unknown type of Selenium action passed to WebElement.RetryAction");
                    }
                }
                catch (ElementNotVisibleException e)
                {
                    retryExceptions.Add(e);
                }
                catch (StaleElementReferenceException e)
                {
                    retryExceptions.Add(e);
                }
                catch (InvalidElementStateException e)
                {
                    retryExceptions.Add(e);
                }
                catch (NoSuchElementException e)
                {
                    retryExceptions.Add(e);
                }
                reFindElement = true;
            }
            if (retryExceptions.Any())
            {
                throw new AggregateException(retryExceptions);
            }
            throw new Exception("Action could not complete for an unknown reason");
        }

        IWebElement ISearchContext.FindElement(By by)
        {
            if (!Initialized)
            {
                WrappedElement = _Driver.FindElement(by);
            }
            return WrappedElement.FindElement(by);
        }

        ReadOnlyCollection<IWebElement> ISearchContext.FindElements(By by)
        {
            if (!Initialized)
            {
                WrappedElement = _Driver.FindElement(by);
            }
            return WrappedElement.FindElements(by);
        }
    }
}
