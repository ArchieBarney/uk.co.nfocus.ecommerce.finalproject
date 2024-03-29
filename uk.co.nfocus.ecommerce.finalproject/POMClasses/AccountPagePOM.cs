﻿using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static uk.co.nfocus.ecommerce.finalproject.Utils.StaticWaitHelper;

namespace uk.co.nfocus.ecommerce.finalproject.POMClasses
{
    internal class AccountPagePOM
    {
        private IWebDriver _driver;

        public AccountPagePOM(IWebDriver driver)
        {
            this._driver = driver;
        }

        private IWebElement _usernameField => _driver.FindElement(By.Id("username"));

        private IWebElement _passwordField => _driver.FindElement(By.Id("password"));

        private IWebElement _loginButton => _driver.FindElement(By.CssSelector("button[value='Log in']"));

        private IWebElement _shopButton => _driver.FindElement(By.PartialLinkText("Shop"));

        private IWebElement _accountOrders => _driver.FindElement(By.PartialLinkText("Orders"));

        private IWebElement _topAccountOrderNum => _driver.FindElement(By.CssSelector("tbody :first-child :first-child a"));

        public string Username
        {
            set
            {
                _usernameField.Clear();
                _usernameField.SendKeys(value);
            }
        }

        public string Password
        {
            set
            {
                _passwordField.Clear();
                _passwordField.SendKeys(value);
            }
        }

        public string Account_Order_Num => _topAccountOrderNum.Text;

        public void AccountLogin()
        {
            _loginButton.Click();
        }

        public void ShopNavigate()
        {
            _shopButton.Click();
        }

        public void GoToAccountOrders()
        {
            _accountOrders.Click();
        }

        public IWebElement GetAccountOrders()
        {
            return _accountOrders;
        }
    }
}
