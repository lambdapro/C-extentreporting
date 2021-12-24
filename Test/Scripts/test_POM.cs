using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using POMExample.PageObjects;
using NUnit.Framework;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System.Text;
using OpenQA.Selenium.Chrome;
using System.IO;
using NUnit.Framework.Interfaces;

namespace POMExample
{
    [TestFixture("chrome", "86.0", "Windows 10")]
    [TestFixture("internet explorer", "11.0", "Windows 10")]
    [TestFixture("Safari", "11.0", "macOS High Sierra")]

    [Parallelizable(ParallelScope.All)]

    public class ExtentReportTests
    {
        String search_string = "LambdaTest";
        String web_page_title = "Google";
        ThreadLocal<IWebDriver> driver = new ThreadLocal<IWebDriver>();
        private String browser;
        private String version;
        private String os;

        public static ExtentReports _extent;
        public ExtentTest _test;
        public String TC_Name;

        public ExtentReportTests(String browser, String version, String os)
        {
            this.browser = browser;
            this.version = version;
            this.os = os;
        }

        [OneTimeSetUp]
        protected void ExtentStart()
        {
            var path = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
            var actualPath = path.Substring(0, path.LastIndexOf("bin"));
            var projectPath = new Uri(actualPath).LocalPath;
            Directory.CreateDirectory(projectPath.ToString() + "Reports");

            Console.WriteLine(projectPath.ToString());
            var reportPath = projectPath + "Reports\\Index.html";
            Console.WriteLine(reportPath);
            /* For Version 3 */
            /* var htmlReporter = new ExtentV3HtmlReporter(reportPath); */
            /* For version 4 --> Creates Index.html */
            var htmlReporter = new ExtentHtmlReporter(reportPath);
            _extent = new ExtentReports();
            _extent.AttachReporter(htmlReporter);
            _extent.AddSystemInfo("Host Name", "Cloud-based Selenium Grid on LambdaTest");
            _extent.AddSystemInfo("Environment", "Test Environment");
            _extent.AddSystemInfo("UserName", "Himanshu Sheth");
            htmlReporter.LoadConfig(projectPath + "report-config.xml");
        }

        [SetUp]
  
        public void Init()
        {
            String username = "";
            String accesskey = "";
            String gridURL = "@hub.lambdatest.com/wd/hub";

            ChromeOptions chromeOptions = new ChromeOptions();


            driver.Value = new RemoteWebDriver(new Uri("https://" + username + ":" + accesskey + gridURL), chromeOptions);

            System.Threading.Thread.Sleep(2000);
        }

        [Test]
        public void SearchLT_Google()
        {
            String test_url = "https://www.google.com";
            String expected_PageTitle = "Most Powerful Cross Browser Testing Tool Online | LambdaTest";
            String result_PageTitle;
            Console.WriteLine("SearchLT_Google");

            String context_name = TestContext.CurrentContext.Test.Name + " on " + browser + " " + version + " " + os;
            TC_Name = context_name;

            _test = _extent.CreateTest(context_name);

            HomePage home_page = new HomePage(driver.Value);
            home_page.goToPage(test_url);
            home_page.test_search(search_string);

            SearchPage search_page = new SearchPage(driver.Value); ;
            FinalPage final_page = search_page.click_search_results();

            result_PageTitle = final_page.getPageTitle();

            final_page.load_complete();
            Assert.AreEqual(result_PageTitle, expected_PageTitle, "Search Test Passed");
        }

        [OneTimeTearDown]
        protected void ExtentClose()
        {
            Console.WriteLine("OneTimeTearDown");
            _extent.Flush();
        }

        [TearDown]
        public void Cleanup()
        {
            bool passed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;
            var exec_status = TestContext.CurrentContext.Result.Outcome.Status;
            var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace) ? ""
            : string.Format("{0}", TestContext.CurrentContext.Result.StackTrace);
            Status logstatus = Status.Pass;
            String screenShotPath, fileName;

            Console.WriteLine("TearDown");

            DateTime time = DateTime.Now;
            fileName = "Screenshot_" + time.ToString("h_mm_ss") + TC_Name + ".png";

            switch (exec_status)
            {
                case TestStatus.Failed:
                    logstatus = Status.Fail;
                    /* The older way of capturing screnshots */
                    screenShotPath = Capture(driver.Value, fileName);
                    /* Capturing Screenshots using built-in methods in ExtentReports 4 */
                    var mediaEntity = CaptureScreenShot(driver.Value, fileName);
                    _test.Log(Status.Fail, "Fail");
                    /* Usage of MediaEntityBuilder for capturing screenshots */  
                    _test.Fail("ExtentReport 4 Capture: Test Failed", mediaEntity);
                    /* Usage of traditional approach for capturing screnshots */
                    _test.Log(Status.Fail, "Traditional Snapshot below: " + _test.AddScreenCaptureFromPath("Screenshots\\" + fileName));
                    break;
                case TestStatus.Passed:
                    logstatus = Status.Pass;
                    /* The older way of capturing screnshots */
                    screenShotPath = Capture(driver.Value, fileName);
                    /* Capturing Screenshots using built-in methods in ExtentReports 4 */
                    mediaEntity = CaptureScreenShot(driver.Value, fileName);
                    _test.Log(Status.Pass, "Pass");
                    /* Usage of MediaEntityBuilder for capturing screenshots */
                    _test.Pass("ExtentReport 4 Capture: Test Passed", mediaEntity);
                    /* Usage of traditional approach for capturing screnshots */
                    _test.Log(Status.Pass, "Traditional Snapshot below: " + _test.AddScreenCaptureFromPath("Screenshots\\" + fileName));
                    break;
                case TestStatus.Inconclusive:
                    logstatus = Status.Warning;
                    break;
                case TestStatus.Skipped:
                    logstatus = Status.Skip;
                    break;
                default:
                    break;
            }
            _test.Log(logstatus, "Test: " + TC_Name + " Status:" + logstatus + stacktrace);

            try
            {
                ((IJavaScriptExecutor)driver.Value).ExecuteScript("lambda-status=" + (passed ? "passed" : "failed"));
            }
            finally
            {
                driver.Value.Quit();
            }
        }

        public static string Capture(IWebDriver driver, String screenShotName)
        {
            ITakesScreenshot ts = (ITakesScreenshot)driver;
            Screenshot screenshot = ts.GetScreenshot();
            var pth = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
            var actualPath = pth.Substring(0, pth.LastIndexOf("bin"));
            var reportPath = new Uri(actualPath).LocalPath;
            Directory.CreateDirectory(reportPath + "Reports\\" + "Screenshots");
            var finalpth = pth.Substring(0, pth.LastIndexOf("bin")) + "Reports\\Screenshots\\" + screenShotName;
            var localpath = new Uri(finalpth).LocalPath;
            screenshot.SaveAsFile(localpath, ScreenshotImageFormat.Png);
            return reportPath;
        }

        public MediaEntityModelProvider CaptureScreenShot(IWebDriver driver, String screenShotName)
        {
            ITakesScreenshot ts = (ITakesScreenshot)driver;
            var screenshot = ts.GetScreenshot().AsBase64EncodedString;

            return MediaEntityBuilder.CreateScreenCaptureFromBase64String(screenshot, screenShotName).Build();
        }
    }
}
