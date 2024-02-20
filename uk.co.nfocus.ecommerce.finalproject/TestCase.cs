using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uk.co.nfocus.ecommerce.finalproject.POMClasses;
using uk.co.nfocus.ecommerce.finalproject.Utils;

using static uk.co.nfocus.ecommerce.finalproject.Utils.StaticWaitHelper;

namespace uk.co.nfocus.ecommerce.finalproject
{
    internal class TestCase : BaseClass
    {
        [Test, Category("Shipping + Coupon")]
        public void TestCaseOne()
        {
            // Dismiss bottom link to prevent intercepted web elements
            _driver.FindElement(By.CssSelector(".woocommerce-store-notice__dismiss-link")).Click();

            // Get to the account page from home POM
            HomePagePOM home = new(_driver);
            home.GoAccountLogin();

            // Use account POM to login to a placeholder account
            AccountPagePOM account = new(_driver)
            {
                Username = Environment.GetEnvironmentVariable("EMAIL"),
                Password = Environment.GetEnvironmentVariable("PASSWORD")
            };
            account.AccountLogin();

            //Empty Basket check
            _driver.FindElement(By.PartialLinkText("Cart")).Click();
            try
            {
                _driver.FindElement(By.CssSelector(".remove")).Click();
            }
            catch (Exception)
            {
                //Do nothing, the basket is already empty
            }

            // Navigate back to shop once basket is emptied
            account.ShopNavigate();

            // Add items to cart and view cart
            ShopPagePOM shop = new(_driver);
            shop.AddItemToCart();
            shop.VeiwCart();

            // Apply coupon using POM
            CartPagePOM cart = new(_driver)
            {
                coupon = "edgewords"
            };
            cart.ApplyCoupon();

            // Wait to ensure coupon value appears on web page
            StaticWaitForElement(_driver, By.CssSelector("td[data-title='Coupon: edgewords'] span[class='woocommerce-Price-amount amount']"));

            // Collecting values for assert to check coupon is applied
            decimal subTotal = Convert.ToDecimal(cart.Sub_total.Remove(0, 1));
            decimal couponTotal = Convert.ToDecimal(cart.Coupon_total.Remove(0, 1));

            try
            {
                Assert.That(Decimal.Multiply(subTotal, (decimal)0.15), Is.EqualTo(couponTotal));
            }
            catch (Exception)
            {
                throw new Exception("Coupon has not applied sufficient 15% off subtotal");
            }

            // Reporting for the coupon assertion
            ScrollElementIntoView(_driver, cart.GetFinalTotal());
            string couponScreenshot = ScreenshotElement(_driver, "coupon.png");
            Console.WriteLine("Coupon has applied 15%");
            TestContext.AddTestAttachment(couponScreenshot);

            // Collecting values for assert to check shipping and coupon is applied to total
            decimal shippingCost = Convert.ToDecimal(cart.Shipping_total.Remove(0, 1));
            decimal finalTotal = Convert.ToDecimal(cart.Final_total.Remove(0, 1));

            try
            {
                Assert.That(subTotal - couponTotal + shippingCost, Is.EqualTo(finalTotal));
            }
            catch (Exception)
            {
                throw new Exception("Final total is not properly calculated, make sure shipping" +
                                    " cost is applied and any coupon discounts are sufficiently applied");
            }

            // Reporting for the shipping + total assertion
            ScrollElementIntoView(_driver, cart.GetFinalTotal());
            string totalScreenshot = ScreenshotElement(_driver, "total.png");
            Console.WriteLine("Total has been sufficiently calculated");
            TestContext.AddTestAttachment(totalScreenshot);

            // Logout of account
            home.GoAccountLogin();
            home.Logout();
        }
    }
}
