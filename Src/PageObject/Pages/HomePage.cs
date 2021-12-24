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
    class HomePage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public HomePage(IWebDriver driver)
        {
            this.driver = driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            PageFactory.InitElements(driver, this);
        }

        [FindsBy(How = How.Name, Using = "q")]
        [CacheLookup]
        private IWebElement elem_search_text;

        [FindsBy(How = How.Name, Using = "btnI")]
        [CacheLookup]
        private IWebElement elem_submit_button;

        [FindsBy(How = How.Id, Using = "hplogo")]
        [CacheLookup]
        private IWebElement elem_logo_img;

        public void goToPage(String test_url)
        {
            driver.Navigate().GoToUrl(test_url);
        }

        // Returns the Page Title
        public String getPageTitle()
        {
            return driver.Title;
        }

        // Returns the search string
        public String getSearchText()
        {
            return elem_search_text.Text;
        }

        // Checks whether the Logo is displayed properly or not
        public bool getWebPageLogo()
        {
            return elem_logo_img.Displayed;
        }

        public SearchPage test_search(string input_search)
        {
            elem_search_text.SendKeys(input_search);
            elem_search_text.Submit();
            return new SearchPage(driver);
        }
    }
}
