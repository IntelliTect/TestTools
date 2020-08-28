using IntelliTect.TestTools.Selenate;
using OpenQA.Selenium;
using System;

namespace GoogleSearch
{
    public class GooglePage
    {
        public GooglePage(IWebDriver driver)
        {
            Driver = driver;
        }

        public static Uri URL { get; } = new Uri("https://www.google.com");
        public FrontPage FrontPage => new FrontPage(Driver);
        public ResultsPage ResultsPage => new ResultsPage(Driver);

        private IWebDriver Driver { get; }
    }

    public class FrontPage
    {
        public FrontPage(IWebDriver driver)
        {
            Driver = driver;
        }

        private IWebDriver Driver { get; }

        public static Uri URL { get; } = new Uri("https://www.google.com");

        // Below selector works as of 9/16/2019. Monitor to see if the first div selector changes when the doodle changes
        public static By SeachButtonBy => By.CssSelector("div[class^='FPdoLc'] input[aria-label='Google Search']");
        public static By SearchInputBy => By.CssSelector("input[title='Search']");

        public ElementHandler SearchInput => new ElementHandler(Driver, SearchInputBy);
        public ElementHandler SeachButton => new ElementHandler(Driver, SeachButtonBy);

    }

    public static class ResultsPageBys
    {

    }

    public class ResultsPage
    {
        public ResultsPage(IWebDriver driver)
        {
            Driver = driver;
        }

        private IWebDriver Driver { get; }

        public static By ResultsDivBy => By.CssSelector("div[data-async-context^='query:']");
        public static By ResultsHeaderBy => By.CssSelector("div[id='rso'] div[class='g'] div[class='rc'] h3");
        public static By GoHomeButtonBy => By.CssSelector("div.logo a");

        public ElementHandler SearchResultsDiv => new ElementHandler(Driver, ResultsDivBy);
        public ElementsHandler SearchResultsHeadersList => new ElementsHandler(Driver, ResultsHeaderBy);
        public ElementHandler GoHomeButton => new ElementHandler(Driver, GoHomeButtonBy);
    }

    
}
