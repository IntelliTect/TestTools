using OpenQA.Selenium;
using IntelliTect.TestTools.Selenate;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace GoogleSearch
{
    public class GoogleHarness
    {
        public GoogleHarness(GoogleBrowser browser)
        {
            Browser = browser;
        }

        public static Uri URL => new Uri("https://www.google.com");

        // Below selector works as of 9/16/2019. Monitor to see if the first div selector changes when the doodle changes
        public IWebElement GoogleSearchButton => Browser.FindElement(By.CssSelector("div[class^='FPdoLc'] input[aria-label='Google Search']"));
        public IWebElement SearchInput => Browser.FindElement(By.CssSelector("input[title='Search']"));
        public IWebElement SearchResultsDiv =>
                Browser.FindElement(By.CssSelector("div[data-async-context^='query:']"));
        public IReadOnlyCollection<IWebElement> SearchResultsHeadersList => 
            Browser.FindElements(By.CssSelector("div[id='rso'] div[class='g'] div[class='rc'] h3"));
        public IWebElement GoHomeButton => Browser.FindElement(By.CssSelector("div.logo a"));

        private GoogleBrowser Browser { get; }
    }
}
