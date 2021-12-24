using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using NUnit.Framework;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;

namespace POMExample.PageObjects
{
    class FinalPage
    {
        private IWebDriver driver;
        Int32 timeout = 10000; // in milliseconds

        public FinalPage(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
        }

        [FindsBy(How = How.XPath, Using = "/html/body/div[1]/header/div[3]/nav/a/img")]
        private IWebElement elem_lt_logo;

        public String getPageTitle()
        {
            return driver.Title;
        }

        // Checks whether the LambdaTest Logo is displayed properly or not
        public bool getLTPageLogo()
        {
            return elem_lt_logo.Displayed;
        }

        public void load_complete()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeout));

            // Wait for the page to load
            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
        }
    }
}
