namespace IntelliTect.TestTools.Selenate.Examples.Pages;

internal class ShadowDomPage
{
    public ShadowDomPage(IWebDriver driver)
    {
        Driver = driver;
    }

    public ElementHandler ShadowRootElement => new ElementHandler(Driver, By.CssSelector("my-paragraph"));
    public ISearchContext ShadowRootContext => ShadowRootElement.GetWebElement().GetShadowRoot();
    public ElementHandler OriginalText => new ElementHandler(Driver, By.CssSelector("slot[name='my-text']")).SetSearchContext(ShadowRootContext);
    public ElementHandler DisplayedText => new ElementHandler(Driver, By.CssSelector("span[slot='my-text']"));

    private IWebDriver Driver { get; }
}
