using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static uk.co.nfocus.ecommerce.finalproject.Utils.StaticWaitHelper;

namespace uk.co.nfocus.ecommerce.finalproject.POMClasses
{
    internal class ShopPagePOM
    {
        private IWebDriver _driver;

        public ShopPagePOM(IWebDriver driver)
        {
            this._driver = driver;
        }

        private IWebElement _addToCart => _driver.FindElement(By.CssSelector("a[aria-label='Add “Beanie” to your cart']"));

        private IWebElement _veiwCart => new WebDriverWait(_driver, TimeSpan.FromSeconds(3)).Until(drv => drv.FindElement(By.CssSelector("a[title='View cart']")));

        public void AddItemToCart()
        {
            _addToCart.Click();
        }

        public void VeiwCart()
        {
            StaticWaitForElement(_driver, By.CssSelector("a[title='View cart']"));
            _veiwCart.Click();
        }
    }
}
