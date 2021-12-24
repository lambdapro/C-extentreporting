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
    class SearchPage
    {
        private IWebDriver driver;
        Int32 timeout = 10000; // in milliseconds

        public SearchPage(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
        }

        [FindsBy(How = How.XPath, Using = "//span[.='LambdaTest: Most Powerful Cross Browser Testing Tool Online']")]
        private IWebElement elem_first_result;

        async void async_delay()
        {
            await Task.Delay(50);
        }

        public FinalPage click_search_results()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeout));

            // Wait for the page to load
            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

            elem_first_result.Click();

            async_delay();

            return new FinalPage(driver);
        }
    }
}
