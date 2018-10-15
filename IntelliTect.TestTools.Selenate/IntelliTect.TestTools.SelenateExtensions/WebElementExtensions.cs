﻿using System;
using IntelliTect.TestTools.Selenate;
using OpenQA.Selenium;
using System.Threading.Tasks;

namespace IntelliTect.TestTools.SelenateExtensions
{
    public static class WebElementExtensions
    {
        public static void FindElementWhenReady(this IWebElement element, By by)
        {

        }

        public static void FindElementsWhenReady(this IWebElement element, By by)
        {

        }

        public static void ScrollIntoView(this IWebElement element, IWebDriver driver, int pixelsFromTopOfScreen = 200)
        {
            int position = element.Location.Y - pixelsFromTopOfScreen;
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript($"window.scrollTo(0,{position})");
        }

        public static async Task FillInWithAndTabWhenReady(this IWebElement element, string value)
        {
            await element.FillInWithWhenReady(value);
            return element.SendKeys(Keys.Tab);
        }

        public static async Task FillInWithWhenReady(this IWebElement element, string value, int secondstoTry = 5)
        {
            var count = 0;
            ConditionalWait wait = new ConditionalWait();
            while (element.GetAttribute("value") != value && count < 5)
            {
                await wait.WaitFor<NoSuchElementException, 
                    ElementNotVisibleException>(
                    () => element.Clear(), TimeSpan.FromSeconds(secondstoTry));
                await wait.WaitFor<NoSuchElementException, 
                    ElementNotVisibleException, 
                    ElementNotInteractableException>(
                    () => element.SendKeys(value), TimeSpan.FromSeconds(secondstoTry));
                count++;
            }
            return null;
        }

        public static Task ClickWhenReady( this IWebElement element, int secondsToTry = 5 )
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitFor<
                NoSuchElementException,
                ElementNotVisibleException,
                ElementClickInterceptedException>( () => element.Click(), TimeSpan.FromSeconds( secondsToTry ) );
        }
    }
}
