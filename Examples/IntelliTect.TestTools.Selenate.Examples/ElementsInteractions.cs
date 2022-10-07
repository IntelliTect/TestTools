namespace IntelliTect.TestTools.Selenate.Examples;

public class ElementsInteractions : TestBase
{
    public ElementsInteractions()
    {
        _ChallengingDomPage = new ChallengingDomPage(WebDriver);
    }

    private ChallengingDomPage _ChallengingDomPage;

    [Fact]
    public void GetASingleElementFromCollection()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/challenging_dom");
        string textToFind = "Iuvaret0";
        IWebElement foundElem = _ChallengingDomPage.FirstRow.GetSingleWebElement(x => x.Text == textToFind);
        // Make sure GetSingleElement actually returned the expected element.
        Assert.Equal(
            textToFind,
            foundElem.Text);
    }

    [Fact]
    public void GetAListOfElementsFromCollection()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/challenging_dom");
        int foundCount = _ChallengingDomPage.Headers.GetAllWebElements(x => x.Displayed).Count;
        Assert.Equal(
            7,
            foundCount);
    }
}
