// FindDefendantNavigation
using legallead.records.search.Classes;
using legallead.records.search.Models;
using OpenQA.Selenium;

namespace legallead.records.search.Addressing
{
    public class FindDefendantNavigation : FindDefendantBase
    {
        public override bool CanFind { get; set; }

        public override void Find(IWebDriver driver, HLinkDataRow linkData)
        {
            if (driver == null)
            {
                throw new System.ArgumentNullException(nameof(driver));
            }

            if (linkData == null)
            {
                throw new System.ArgumentNullException(nameof(linkData));
            }

            CanFind = false;
            ElementAssertion helper = new(driver);
            helper.Navigate(linkData.WebAddress);
            driver.WaitForNavigation();
            // get criminal hyperlink
            // //a[contains(text(),'Criminal')]
            IWebElement? criminalLink = TryFindElement(driver, By.XPath(CommonKeyIndexes.CriminalLinkXpath));
            IWebElement? elementCaseName = TryFindElement(driver, By.XPath(CommonKeyIndexes.CaseStlyeBoldXpath));
            if (criminalLink != null)
            {
                if (elementCaseName != null)
                {
                    linkData.CriminalCaseStyle = elementCaseName.Text;
                }
                linkData.IsCriminal = true;
            }
            if (linkData.IsJustice && elementCaseName != null)
            {
                linkData.CriminalCaseStyle = elementCaseName.Text;
            }
            linkData.PageHtml =
                GetTable(driver, By.XPath(@"//div[contains(text(),'Party Information')]"));
        }

        private static string GetTable(IWebDriver driver, By by)
        {
            try
            {
                IWebElement dv = driver.FindElement(by);
                IWebElement parent = dv.FindElement(By.XPath(IndexKeyNames.ParentElement));
                while (parent.TagName != "table")
                {
                    parent = parent.FindElement(By.XPath(IndexKeyNames.ParentElement));
                }
                return parent.GetAttribute("outerHTML");
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}