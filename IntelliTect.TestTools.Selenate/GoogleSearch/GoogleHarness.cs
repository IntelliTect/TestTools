using OpenQA.Selenium;
using System;

namespace GoogleSearch
{
    public static class GoogleHarness
    {
        public static Uri URL { get; } = new Uri("https://www.google.com");

        // Below selector works as of 9/16/2019. Monitor to see if the first div selector changes when the doodle changes
        public static By GoogleSearchButton => By.CssSelector("div[class^='FPdoLc'] input[aria-label='Google Search']");
        public static By SearchInput => By.CssSelector("input[title='Search']");
        public static By SearchResultsDiv => By.CssSelector("div[data-async-context^='query:']");
        public static By SearchResultsHeadersList => By.CssSelector("div[id='rso'] div[class='g'] div[class='rc'] h3");
        public static By GoHomeButton => By.CssSelector("div.logo a");
    }
}
