using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uk.co.nfocus.ecommerce.finalproject.Utils
{
    internal static class StaticWaitHelper
    {
        public static void StaticWaitForElement(IWebDriver driver, By locator, int timeInSeconds = 3)
        {
            WebDriverWait myWait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeInSeconds));
            myWait.Until(drv => drv.FindElement(locator).Enabled);
        }

        public static void ScrollElementIntoView(IWebDriver driver, IWebElement element)
        {
            IJavaScriptExecutor? jsdriver = driver as IJavaScriptExecutor;

            if(jsdriver != null)
            {
                jsdriver.ExecuteScript("arguments[0].scrollIntoView()", element);
            }
        }

        public static string ScreenshotElement(IWebDriver driver, string nameOfScreenshot)
        {
            Screenshot screenshot = driver.TakeScreenshot();
            screenshot.SaveAsFile(Directory.GetCurrentDirectory() + nameOfScreenshot);
            return Directory.GetCurrentDirectory() + nameOfScreenshot;
        }
    }
}
