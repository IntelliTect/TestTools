namespace IntelliTect.TestTools.Selenate.Examples.Pages;

public class ChallengingDomPage
{
    public ChallengingDomPage(IWebDriver driver)
    {
        _Driver = driver ?? throw new ArgumentNullException(nameof(driver));
    }

    // NOTE: Tables are a known deficiency in Selenate.
    // An upcoming feature update will include better support for this.
    public ElementHandler Table => new(_Driver, By.CssSelector("table"));
    public ElementsHandler Headers => Table.FindElements(By.CssSelector("thead th"));
    public ElementsHandler FirstRow => Table.FindElements(By.CssSelector("tbody>tr:nth-of-type(1)>td"));

    private IWebDriver _Driver;
}