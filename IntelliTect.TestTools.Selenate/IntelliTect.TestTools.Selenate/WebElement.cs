using OpenQA.Selenium;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System;

namespace IntelliTect.TestTools.Selenate
{
    public class WebElement : IWebElement
    {
        // Move this class to Extensions at some point. This is really no longer needed with how easy it is to call waits now. This would just be for people that want an "out of the box" implementation
        // Also: do we ever need to re-find the element if an operation throws a stale element reference?
        //We were doing that in the prior impelmentation, but I'm not sure actually sure it's needed since we're making sure the element is found before doing anything with it.
        public WebElement(IWebElement wrappedElement, /*By by,*/ IWebDriver driver)
        {
            WrappedElement = wrappedElement;
            //By = by;
            _Driver = driver;
        }

        public By By { get; }

        string IWebElement.TagName => TagName.ConfigureAwait(false).GetAwaiter().GetResult();
        public Task<string> TagName
        {
            get
            {
                ConditionalWait wait = new ConditionalWait();
                return wait.WaitFor<StaleElementReferenceException, string>(() => WrappedElement.TagName, TimeSpan.FromSeconds(15));
            }
        }

        string IWebElement.Text => Text.GetAwaiter().GetResult();
        public Task<string> Text
        {
            get
            {
                ConditionalWait wait = new ConditionalWait();
                return wait.WaitFor<StaleElementReferenceException, string>(() => WrappedElement.Text, TimeSpan.FromSeconds(15));
            }
        }

        bool IWebElement.Enabled => Enabled.ConfigureAwait(false).GetAwaiter().GetResult();
        public Task<bool> Enabled
        {
            get
            {
                ConditionalWait wait = new ConditionalWait();
                return wait.WaitFor<StaleElementReferenceException, bool>(() => WrappedElement.Enabled, TimeSpan.FromSeconds(15));
            }
        }

        bool IWebElement.Displayed => Displayed.ConfigureAwait(false).GetAwaiter().GetResult();
        public Task<bool> Displayed
        {
            get
            {
                ConditionalWait wait = new ConditionalWait();
                return wait.WaitFor<StaleElementReferenceException, bool>(() => WrappedElement.Displayed, TimeSpan.FromSeconds(15));
            }
        }

        bool IWebElement.Selected => Selected.ConfigureAwait(false).GetAwaiter().GetResult();
        public Task<bool> Selected
        {
            get
            {
                ConditionalWait wait = new ConditionalWait();
                return wait.WaitFor<StaleElementReferenceException, bool>(() => WrappedElement.Selected, TimeSpan.FromSeconds(15));
            }
        }

        Point IWebElement.Location => Location.ConfigureAwait(false).GetAwaiter().GetResult();
        public Task<Point> Location
        {
            get
            {
                ConditionalWait wait = new ConditionalWait();
                return wait.WaitFor<StaleElementReferenceException, Point>(() => WrappedElement.Location, TimeSpan.FromSeconds(15));
            }
        }

        Size IWebElement.Size => Size.ConfigureAwait(false).GetAwaiter().GetResult();
        public Task<Size> Size
        {
            get
            {
                ConditionalWait wait = new ConditionalWait();
                return wait.WaitFor<StaleElementReferenceException, Size>(() => WrappedElement.Size, TimeSpan.FromSeconds(15));
            }
        }

        IWebElement ISearchContext.FindElement(By by)
        {
            return FindElement(by).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task<WebElement> FindElement(By by)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitFor<StaleElementReferenceException, WebElement>(() => new WebElement(WrappedElement.FindElement(by), _Driver), TimeSpan.FromSeconds(15));
        }

        ReadOnlyCollection<IWebElement> ISearchContext.FindElements(By by)
        {
            return WrappedElement.FindElements(by);
        }

        public Task<ReadOnlyCollection<WebElement>> FindElements(By by)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitFor<StaleElementReferenceException, ReadOnlyCollection<WebElement>>(() => new ReadOnlyCollection<WebElement>(
                    WrappedElement.FindElements(by)
                        .Select(webElement =>
                        new WebElement(WrappedElement.FindElement(by), _Driver)).ToList()), TimeSpan.FromSeconds(15));
        }

        void IWebElement.Clear()
        {
            Clear().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        public Task Clear()
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitFor<StaleElementReferenceException>(() => WrappedElement.Clear(), TimeSpan.FromSeconds(15));
        }

        void IWebElement.SendKeys(string text)
        {
            SendKeys(text).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task SendKeys(string text)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitFor<StaleElementReferenceException>(() => WrappedElement.SendKeys(text), TimeSpan.FromSeconds(15));
        }

        void IWebElement.Submit()
        {
            Submit().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task Submit()
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitFor<StaleElementReferenceException>(() => WrappedElement.Submit(), TimeSpan.FromSeconds(15));
        }

        void IWebElement.Click()
        {
            Click().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task Click()
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitFor<StaleElementReferenceException, ElementNotVisibleException>(() => WrappedElement.Click(), TimeSpan.FromSeconds(15));
        }

        // Can this easily be abstracted out to an extension method?
        // Seems to make more sense as an extension
        public Task Click(int secondsToRetry)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitFor<StaleElementReferenceException, ElementNotVisibleException>(() => WrappedElement.Click(), TimeSpan.FromSeconds(secondsToRetry));
        }

        string IWebElement.GetAttribute(string attributeName)
        {
            return GetAttribute(attributeName).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task<string> GetAttribute(string attributeName)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitFor<StaleElementReferenceException, string>(() => WrappedElement.GetAttribute(attributeName), TimeSpan.FromSeconds(15));
        }

        string IWebElement.GetProperty(string propertyName)
        {
            return GetProperty(propertyName).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task<string> GetProperty(string propertyName)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitFor<StaleElementReferenceException, string>(() => WrappedElement.GetProperty(propertyName), TimeSpan.FromSeconds(15));
        }

        string IWebElement.GetCssValue(string propertyName)
        {
            return GetCssValue(propertyName).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task<string> GetCssValue(string propertyName)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitFor<StaleElementReferenceException, string>(() => WrappedElement.GetCssValue(propertyName), TimeSpan.FromSeconds(15));
        }

        private IWebElement WrappedElement { get; set; }
        private readonly IWebDriver _Driver;
    }
}
