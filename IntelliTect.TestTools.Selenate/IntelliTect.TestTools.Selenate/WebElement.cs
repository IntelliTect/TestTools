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
        public WebElement(By by, IWebDriver driver)
        {
            By = by;
            _Driver = driver;
        }

        public WebElement(IWebElement wrappedElement, By by, IWebDriver driver)
        {
            WrappedElement = wrappedElement;
            By = by;
            _Driver = driver;
        }

        public By By { get; }
        public bool Initialized => WrappedElement != null;

        public string TagName => WrappedElement.TagName;
        public string Text => RetryAction("text");
        public bool Enabled => Convert.ToBoolean(RetryAction("enabled"));
        public bool Displayed => Convert.ToBoolean(RetryAction("displayed"));
        public bool Selected => Convert.ToBoolean(RetryAction("selected"));
        public Point Location => WrappedElement.Location;
        public Size Size => WrappedElement.Size;

        public WebElement FindElement(By by)
        {
            if (!Initialized)
            {
                WrappedElement = _Driver.FindElement(by);
            }

            WebDriverWait wait = new WebDriverWait(_Driver, TimeSpan.FromSeconds(5));
            try
            {
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(by));
            }
            catch (WebDriverTimeoutException)
            {
                return new WebElement(by, _Driver);
            }

            return new WebElement(WrappedElement.FindElement(by), by, _Driver);
        }

        public ReadOnlyCollection<WebElement> FindElements(By by)
        {
            if (!Initialized)
            {
                WrappedElement = _Driver.FindElement(by);
            }

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

        public void Clear()
        {
            RetryAction("clear");
        }

        public void SendKeys(string text)
        {
            RetryAction("sendkeys", text);
        }

        public void Submit()
        {
            RetryAction("submit");
        }

        public void Click()
        {
            RetryAction("click");
        }

        public void Click(int timeToRetry)
        {
            TimeToRetry = timeToRetry;
            Click();
            TimeToRetry = DefaultRetryTime;
        }

        public string GetAttribute(string attributeName)
        {
            return RetryAction("getattribute", attributeName);
        }

        public string GetProperty(string propertyName)
        {
            return RetryAction("getproperty", propertyName);
        }

        public string GetCssValue(string propertyName)
        {
            return RetryAction("getcss", propertyName);
        }

        private const int DefaultRetryTime = 30;
        private int TimeToRetry { get; set; } = DefaultRetryTime;
        private IWebElement WrappedElement { get; set; }
        private readonly IWebDriver _Driver;

        private string RetryAction(string action, string text = null)
        {
            DateTime end = DateTime.Now.AddSeconds(TimeToRetry);
            bool reFindElement = WrappedElement == null;
            List<Exception> retryExceptions = new List<Exception>();
            while (DateTime.Now <= end)
            {
                Task.Delay(250).Wait();
                try
                {
                    if (reFindElement)
                    {
                        WrappedElement = _Driver.FindElement(By);
                        reFindElement = false;
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
                        default:
                            throw new InvalidOperationException(
                                "Unknown type of Selenium action passed to WebElement.RetryAction");
                    }
                }
                catch (ElementNotVisibleException e)
                {
                    retryExceptions.Add(e);
                    reFindElement = true;
                }
                catch (StaleElementReferenceException e)
                {
                    retryExceptions.Add(e);
                    reFindElement = true;
                }
                catch (InvalidElementStateException e)
                {
                    retryExceptions.Add(e);
                    reFindElement = true;
                }
                catch (NoSuchElementException e)
                {
                    retryExceptions.Add(e);
                    reFindElement = true;
                }
            }
            if (retryExceptions.Any())
            {
                throw new AggregateException(retryExceptions);
            }
            throw new Exception("Action could not complete");
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
