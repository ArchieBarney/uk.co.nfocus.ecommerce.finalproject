using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uk.co.nfocus.ecommerce.finalproject.POMClasses;

namespace uk.co.nfocus.ecommerce.finalproject.Utils
{
    internal class BaseClass
    {
        protected IWebDriver _driver;
        [SetUp]
        public void Setup()
        {
            string browser = Environment.GetEnvironmentVariable("BROWSER");

            if(browser == null)
            {
                Console.WriteLine("browser is null, using default (Edge)");
            }

            switch (browser)
            {
                case "chrome":
                    _driver = new ChromeDriver();
                    break;
                case "firefox":
                    _driver = new FirefoxDriver();
                    break;
                case "ie":
                    _driver = new InternetExplorerDriver();
                    break;
                case "firefoxheadless":
                    FirefoxOptions firefoxOptions = new();
                    firefoxOptions.AddArgument("--headless");
                    _driver = new FirefoxDriver(firefoxOptions);
                    break;
                case "chromeheadless":
                    ChromeOptions chromeOptions = new();
                    chromeOptions.AddArgument("--headless");
                    _driver = new ChromeDriver(chromeOptions);
                    break;
                default:
                    _driver = new EdgeDriver();
                    break;
            }

            string startURL = TestContext.Parameters["WebAppURL"];
            if(startURL == null)
            {
                Console.WriteLine("URL provided is null, using default");
                startURL = "https://www.edgewordstraining.co.uk/demo-site/";
            }
            _driver.Url = startURL;
        }

        [TearDown]
        public void Teardown()
        {
            _driver.Quit();
        }
    }
}
