using Newtonsoft.Json;
using OpenQA.Selenium;

namespace ExampleTests.Pages
{
    public class BasePage
    {
        //public BasePage(IWebDriver driver)
        //{
        //    Driver = driver;
        //}

        public string BaseUrl { get; set; } = @"https://intellitect.com/";
        [JsonIgnore]
        public IWebDriver Driver { get; set; }
    }
}
