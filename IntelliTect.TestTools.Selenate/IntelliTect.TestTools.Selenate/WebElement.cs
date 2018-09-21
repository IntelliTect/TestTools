using OpenQA.Selenium;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

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
                return wait.WaitForSeconds<StaleElementReferenceException, string>(() => WrappedElement.TagName);
            }
        }

        string IWebElement.Text => Text.GetAwaiter().GetResult();
        public Task<string> Text
        {
            get
            {
                ConditionalWait wait = new ConditionalWait();
                return wait.WaitForSeconds<StaleElementReferenceException, string>(() => WrappedElement.Text);
            }
        }

        bool IWebElement.Enabled => Enabled.ConfigureAwait(false).GetAwaiter().GetResult();
        public Task<bool> Enabled
        {
            get
            {
                ConditionalWait wait = new ConditionalWait();
                return wait.WaitForSeconds<StaleElementReferenceException, bool>(() => WrappedElement.Enabled);
            }
        }

        bool IWebElement.Displayed => Displayed.ConfigureAwait(false).GetAwaiter().GetResult();
        public Task<bool> Displayed
        {
            get
            {
                ConditionalWait wait = new ConditionalWait();
                return wait.WaitForSeconds<StaleElementReferenceException, bool>(() => WrappedElement.Displayed);
            }
        }

        bool IWebElement.Selected => Selected.ConfigureAwait(false).GetAwaiter().GetResult();
        public Task<bool> Selected
        {
            get
            {
                ConditionalWait wait = new ConditionalWait();
                return wait.WaitForSeconds<StaleElementReferenceException, bool>(() => WrappedElement.Selected);
            }
        }

        Point IWebElement.Location => Location.ConfigureAwait(false).GetAwaiter().GetResult();
        public Task<Point> Location
        {
            get
            {
                ConditionalWait wait = new ConditionalWait();
                return wait.WaitForSeconds<StaleElementReferenceException, Point>(() => WrappedElement.Location);
            }
        }

        Size IWebElement.Size => Size.ConfigureAwait(false).GetAwaiter().GetResult();
        public Task<Size> Size
        {
            get
            {
                ConditionalWait wait = new ConditionalWait();
                return wait.WaitForSeconds<StaleElementReferenceException, Size>(() => WrappedElement.Size);
            }
        }

        IWebElement ISearchContext.FindElement(By by)
        {
            return FindElement(by).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task<WebElement> FindElement(By by)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitForSeconds<StaleElementReferenceException, WebElement>(() => new WebElement(WrappedElement.FindElement(by), _Driver));
        }

        ReadOnlyCollection<IWebElement> ISearchContext.FindElements(By by)
        {
            return WrappedElement.FindElements(by);
        }

        public Task<ReadOnlyCollection<WebElement>> FindElements(By by)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitForSeconds<StaleElementReferenceException, ReadOnlyCollection<WebElement>>(() => new ReadOnlyCollection<WebElement>(
                    WrappedElement.FindElements(by)
                        .Select(webElement =>
                        new WebElement(WrappedElement.FindElement(by), _Driver)).ToList()));
        }

        void IWebElement.Clear()
        {
            Clear().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        public Task Clear()
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitForSeconds<StaleElementReferenceException>(() => WrappedElement.Clear());
        }

        void IWebElement.SendKeys(string text)
        {
            SendKeys(text).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task SendKeys(string text)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitForSeconds<StaleElementReferenceException>(() => WrappedElement.SendKeys(text));
        }

        void IWebElement.Submit()
        {
            Submit().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task Submit()
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitForSeconds<StaleElementReferenceException>(() => WrappedElement.Submit());
        }

        void IWebElement.Click()
        {
            Click().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task Click()
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitForSeconds<StaleElementReferenceException, ElementNotVisibleException>(() => WrappedElement.Click());
        }

        // Can this easily be abstracted out to an extension method?
        // Seems to make more sense as an extension
        public Task Click(int secondsToRetry)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitForSeconds<StaleElementReferenceException, ElementNotVisibleException>(() => WrappedElement.Click(), secondsToRetry);
        }

        string IWebElement.GetAttribute(string attributeName)
        {
            return GetAttribute(attributeName).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task<string> GetAttribute(string attributeName)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitForSeconds<StaleElementReferenceException, string>(() => WrappedElement.GetAttribute(attributeName));
        }

        string IWebElement.GetProperty(string propertyName)
        {
            return GetProperty(propertyName).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task<string> GetProperty(string propertyName)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitForSeconds<StaleElementReferenceException, string>(() => WrappedElement.GetProperty(propertyName));
        }

        string IWebElement.GetCssValue(string propertyName)
        {
            return GetCssValue(propertyName).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task<string> GetCssValue(string propertyName)
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitForSeconds<StaleElementReferenceException, string>(() => WrappedElement.GetCssValue(propertyName));
        }

        private IWebElement WrappedElement { get; set; }
        private readonly IWebDriver _Driver;
    }
}
