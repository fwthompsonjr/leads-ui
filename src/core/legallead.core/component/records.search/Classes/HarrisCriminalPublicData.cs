using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Text;

namespace SeleniumTests
{
    [TestClass]
    public class PublicData
    {
        private static IWebDriver? driver;
        private StringBuilder? verificationErrors;

        [ClassInitialize]
        public static void InitializeClass(TestContext testContext)
        {
            driver = new FirefoxDriver();
        }

        [ClassCleanup]
        public static void CleanupClass()
        {
            try
            {
                driver?.Close();
                driver?.Dispose();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
        }

        [TestInitialize]
        public void InitializeTest()
        {
            verificationErrors = new StringBuilder();
        }

        [TestCleanup]
        public void CleanupTest()
        {
            if (verificationErrors == null) return;
            Assert.AreEqual("", verificationErrors.ToString());
        }

        [TestMethod]
        public void ThePublicDataTest()
        {
            try
            {
                if (driver == null) throw new NullReferenceException();
                driver.Navigate().GoToUrl("https://www.hcdistrictclerk.com/Common/e-services/PublicDatasets.aspx");
                driver.FindElement(By.XPath("//div[contains(string(), \"CrimFilingsWithFutureSettings\")]")).Click();
                driver.FindElement(By.XPath("//div[@id='ctl00_ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder2_ContentPlaceHolder2_blah']/table/tbody/tr[58]/td[3]/a/u/b")).Click();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Assert.Inconclusive("unexpected exception.");
                throw;
            }
        }
    }
}