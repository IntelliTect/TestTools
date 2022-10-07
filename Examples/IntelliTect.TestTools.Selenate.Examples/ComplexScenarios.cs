using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Globalization;

namespace IntelliTect.TestTools.Selenate.Examples;

public class ComplexScenarios : TestBase
{
    public ComplexScenarios()
    {
        _Editor = new EditorPage(WebDriver);
        _Slider = new SliderPage(WebDriver);
    }

    private readonly EditorPage _Editor;
    private readonly SliderPage _Slider;

    [Fact]
    public void ComplexWait()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/tinymce");
        // Group together, as each menu click must occur with the prior one in order to succeed.

        Assert.True(_Editor.MenuBar.WaitForDisplayed());

        Assert.True(
            OpenMenu(
                _Editor.FormatMenu.Locator,
                _Editor.FormatsMenu.Locator));

        Assert.True(
            OpenMenu(
                _Editor.FormatsMenu.Locator,
                _Editor.HeadingsMenu.Locator));

        Assert.True(
            OpenMenu(
                _Editor.HeadingsMenu.Locator,
                _Editor.Heading1Option.Locator));

        _Editor.Heading1Option.Click();

        Assert.Equal("Heading 1", _Editor.ParagraphDropDown.Text());

        // Each menu item must be clicked, then assess the next menu item for completeness.
        // If the second item does not appear, the immediately preceding menu item must be re-clicked.
        // Group both calls together in a single wait so Selenium retries both.
        bool OpenMenu(By originalLocator, By secondLocator)
        {
            WebDriverWait wait = new(
            DriverHandler.WrappedDriver,
            TimeSpan.FromSeconds(5));

            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException));

            return wait.Until(x =>
            {
                x.FindElement(originalLocator).Click();
                return x.FindElement(secondLocator).Displayed;
            });
        }
    }

    [Fact]
    public void DragAndDrop()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/horizontal_slider");

        Assert.True(_Slider.Slider.WaitForDisplayed(),
            "Slider did not appear when we expected it to.");

        var slider = _Slider.Slider.GetWebElement();

        Actions actions = new(DriverHandler.WrappedDriver);
        actions.MoveToElement(slider, 0, 0)
            .ClickAndHold()
            .MoveByOffset(slider.Size.Width / 2, 0)
            .Release()
            .Perform();

        decimal sliderNum = Convert.ToDecimal(_Slider.Number.Text(), CultureInfo.InvariantCulture);
        Assert.True(sliderNum > 0,
            $"Expected slider number to be larger than 0, but was actually {sliderNum}");
    }
}
