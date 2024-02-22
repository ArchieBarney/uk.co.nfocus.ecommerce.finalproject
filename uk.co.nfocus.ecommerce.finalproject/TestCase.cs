using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
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
        [Test, Order(1), Category("Shipping + Coupon")]
        public void TestCaseOne()
        {
            HomePagePOM home = TestPrep();

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
            string couponScreenshot = ScrollElementIntoView(_driver, cart.GetCouponAmount(), "coupon.png");
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
            string totalScreenshot = ScrollElementIntoView(_driver, cart.GetFinalTotal(), "total.png");
            Console.WriteLine("Total has been sufficiently calculated");
            TestContext.AddTestAttachment(totalScreenshot);

            // Logout of account
            home.GoAccountLogin();
            home.Logout();
        }

        [Test, Order(2), Category("Order + Billing")]
        public void TestCaseTwo()
        {
            HomePagePOM home = TestPrep();

            // Set the cart up to proceed to checkout
            CartPagePOM cart = new(_driver);
            cart.ProceedToCheckout();

            // Instansiate the checkout class with all the required fields to pass an order
            CheckoutPagePOM checkout = new(_driver)
            {
                firstName = "Archie",
                lastName = "Barnett",
                streetAdress = Environment.GetEnvironmentVariable("STREET"),
                townCity = Environment.GetEnvironmentVariable("TOWN"),
                postcode = Environment.GetEnvironmentVariable("POSTCODE"),
                phone = Environment.GetEnvironmentVariable("PHONE"),
                email = Environment.GetEnvironmentVariable("EMAIL")
            };

            // Seperate reference for stale element (Thread works, web driver wait doesnt work)
            Thread.Sleep(1000);
            var checkPayment = _driver.FindElement(By.CssSelector(".wc_payment_method.payment_method_cheque"));
            checkPayment.Click();

            checkout.PlaceOrder();

            // Wait for the page to load in order to recieve the order number
            StaticWaitForElement(_driver, By.CssSelector("li[class='woocommerce-order-overview__order order'] strong"));
            string checkoutOrderNumber = checkout.Order_Number;
            Console.WriteLine("Order number is: " + checkoutOrderNumber);

            // Go back to the account and get access to the account order history
            home.GoAccountLogin();
            AccountPagePOM account = new AccountPagePOM(_driver);
            account.GoToAccountOrders();

            try
            {
                Assert.That(account.Account_Order_Num.Remove(0, 1), Is.EqualTo(checkoutOrderNumber));
            }catch (Exception)
            {
                throw new Exception("order on checkout does not appear on account");
            }

            // Reporting for the Order number assertion
            string totalScreenshot = ScrollElementIntoView(_driver, account.GetAccountOrders(), "ordernum.png");
            Console.WriteLine("Order number has been recorded and shows on the account");
            TestContext.AddTestAttachment(totalScreenshot);
            
            // Logout of account
            home.GoAccountLogin();
            home.Logout();
        }

        // Seperate function since both tests start almost identically (Return home POM to access logout function)
        public HomePagePOM TestPrep()
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

            return home;
        }
    }
}
