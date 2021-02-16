using IntelliTect.TestTools.Selenate.Examples.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using Xunit;

namespace IntelliTect.TestTools.Selenate.Examples
{
    public class ComplexScenarios : TestBase
    {
        public ComplexScenarios()
        {
            _Editor = new EditorPage(WebDriver);
        }

        private readonly EditorPage _Editor;

        [Fact]
        public void ComplexWait()
        {
            DriverHandler.NavigateToPage("http://the-internet.herokuapp.com/tinymce");
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
                WebDriverWait wait = new WebDriverWait(
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
        public void Actions()
        {

        }
    }
}
