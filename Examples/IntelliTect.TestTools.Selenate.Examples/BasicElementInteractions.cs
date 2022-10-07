namespace IntelliTect.TestTools.Selenate.Examples;

public class BasicElementInteractions : TestBase
{
    public BasicElementInteractions()
    {
        _DynamicLoadingPage = new DynamicLoadingPages(WebDriver);
        _DynamicControlsPage = new DynamicControlsPage(WebDriver);
        _DropDownPage = new DropDownPage(WebDriver);
        _ShadowDomPage = new ShadowDomPage(WebDriver);
    }

    private readonly DynamicLoadingPages _DynamicLoadingPage;
    private readonly DynamicControlsPage _DynamicControlsPage;
    private readonly DropDownPage _DropDownPage;
    private readonly ShadowDomPage _ShadowDomPage;


    // Below two tests should functionally operate the same
    [Fact]
    public void FindElementThatIsUnhiddenAfterPageLoad()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/dynamic_loading/1");

        _DynamicLoadingPage.StartButton.GetWebElement().FindElements(OpenQA.Selenium.By.Id("testing"));

        _DynamicLoadingPage.StartButton.Click();

        Assert.True(
            _DynamicLoadingPage.HelloWorldLabel
                .SetTimeoutSeconds(8)
                .WaitForDisplayed(),
            "Hello World label did not appear when we expected it to.");
    }

    [Fact]
    public void ClearAndSendKeys()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/dynamic_controls");
        _DynamicControlsPage.EnableDisableButton.Click();
        _DynamicControlsPage.TextBox.SendKeys("Hello!");
        Assert.Equal("Hello!", _DynamicControlsPage.TextBox.GetAttribute("value"));
        _DynamicControlsPage.TextBox.Clear();
        Assert.Equal("", _DynamicControlsPage.TextBox.GetAttribute("value"));
    }

    [Fact]
    public void FindElementThatIsCreatedAfterPageLoad()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/dynamic_loading/2");

        _DynamicLoadingPage.StartButton.Click();

        Assert.True(
            _DynamicLoadingPage.HelloWorldLabel
                .SetTimeoutSeconds(8)
                .WaitForDisplayed(),
            "Hello World label did not appear when we expected it to.");
    }

    [Fact]
    public void CheckForVisibleStates()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/dynamic_controls");

        Assert.True(_DynamicControlsPage.Checkbox.WaitForDisplayed());
        _DynamicControlsPage.RemoveAddButton.Click();
        Assert.True(_DynamicControlsPage.Checkbox.WaitForNotDisplayed());
        _DynamicControlsPage.RemoveAddButton.Click();
        Assert.True(_DynamicControlsPage.Checkbox.WaitForDisplayed());
    }

    [Fact]
    public void CheckForElementEnabledStates()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/dynamic_controls");

        Assert.True(_DynamicControlsPage.TextBox.WaitForDisabled());
        _DynamicControlsPage.EnableDisableButton.Click();
        Assert.True(_DynamicControlsPage.TextBox.WaitForEnabled());
        _DynamicControlsPage.EnableDisableButton.Click();
        Assert.True(_DynamicControlsPage.TextBox.WaitForDisabled());
    }

    [Fact]
    public void ManipulateSelectElement()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/dropdown");
        _DropDownPage.DropDownSelect.SelectByText("Option 2");
        Assert.Equal("Option 2", _DropDownPage.DropDownSelect.SelectedOption.Text);
    }

    [Fact]
    public void FindElementsInShadowDom()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/shadowdom");
        string originalText = _ShadowDomPage.OriginalText.Text();
        string displayedText = _ShadowDomPage.DisplayedText.Text();
        Assert.Equal("My default text", originalText);
        Assert.Equal("Let's have some different text!", displayedText);
    }
}
