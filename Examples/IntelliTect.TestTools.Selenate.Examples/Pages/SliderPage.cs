namespace IntelliTect.TestTools.Selenate.Examples.Pages;

public class SliderPage
{
    public SliderPage(IWebDriver driver)
    {
        _Driver = driver ?? throw new ArgumentNullException(nameof(driver));
    }

    public ElementHandler Slider => new ElementHandler(_Driver, By.CssSelector("input[type='range']"));
    public ElementHandler Number => new ElementHandler(_Driver, By.Id("range"));

    private IWebDriver _Driver;
}
