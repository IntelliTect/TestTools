using Newtonsoft.Json;
using OpenQA.Selenium;

namespace ExampleTests.Pages
{
    public class IntelliTect : BasePage
    {
        public IntelliTect(IWebDriver driver) : base(driver)
        {
            HomePage = new HomePage(Driver);
            Blogs = new Blogs(Driver);
        }

        [JsonIgnore]
        public HomePage HomePage { get; set; }
        [JsonIgnore]
        public Blogs Blogs { get; set; }
    }
}
