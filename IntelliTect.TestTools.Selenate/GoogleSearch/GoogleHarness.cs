using OpenQA.Selenium;
using IntelliTect.TestTools.Selenate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleSearch
{
    public class GoogleHarness
    {
        public GoogleHarness(GoogleBrowser browser)
        {
            Browser = browser;
        }

        public string URL => "https://www.google.com";
        public IWebElement GoogleSearchButton => Browser.FindElement(By.Name("btnK"));
        public IWebElement SearchInput => Browser.FindElement(By.CssSelector("input[title='Search']"));
        public IWebElement SearchResultsDiv =>
                Browser.FindElement(By.CssSelector("div[data-async-context^='query:']"));
        public IReadOnlyCollection<IWebElement> SearchResultsHeadersList => 
            Browser.FindElements(By.CssSelector("div[id='rso']>div div[class='g'] div[class='rc']>h3>a")).GetAwaiter().GetResult();
        public Task<WebElement> GoHomeButton => Browser.FindElementAsync(By.CssSelector("div[class='logo']"));

        private GoogleBrowser Browser { get; }
    }
}
