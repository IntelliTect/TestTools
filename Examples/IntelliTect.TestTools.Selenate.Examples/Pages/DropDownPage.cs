using OpenQA.Selenium.Support.UI;

namespace IntelliTect.TestTools.Selenate.Examples.Pages;

public class DropDownPage
{
    public DropDownPage(IWebDriver driver)
    {
        _Driver = driver ?? throw new ArgumentNullException(nameof(driver));
    }

    public ElementHandler DropDown => new ElementHandler(_Driver, By.Id("dropdown"));
    public SelectElement DropDownSelect => new SelectElement(DropDown.GetWebElement());

    private IWebDriver _Driver;
}
