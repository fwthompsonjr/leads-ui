using legallead.records.search.Classes;
using legallead.records.search.Dto;
using OpenQA.Selenium;

namespace legallead.records.search.Web
{
    public class ElementGetHtmlAction : ElementActionBase
    {
        private const string actionName = "get-table-html";

        public override string ActionName => actionName;

        public bool IsProbateSearch { get; private set; }

        public bool IsJusticeSearch { get; private set; }

        public override void Act(NavigationStep item)
        {
            var driver = GetWeb;
            By? selector = GetSelector(item);
            if (driver == null || selector == null) return;
            IWebElement? element = TryGetElement(driver, selector, 5, 500);
            if (element == null) return;
            string outerHtml = element.GetAttribute("outerHTML");
            CollinWebInteractive helper = new();
            outerHtml = helper.RemoveElement(outerHtml, "<img");
            // remove colspan? <colgroup>
            outerHtml = helper.RemoveTag(outerHtml, "colgroup");
            // remove the image tags now

            OuterHtml = outerHtml;
            string probateLinkXpath = CommonKeyIndexes.ProbateLinkXpath;
            string justiceLinkXpath = probateLinkXpath.Replace("'Probate'", "'Justice'");
            IWebElement? probateLink = driver.TryFindElement(By.XPath(probateLinkXpath));
            IWebElement? justiceLocation = driver.TryFindElement(By.XPath(justiceLinkXpath));
            bool isCollinCounty = driver.Url.Contains("co.collin.tx.us");

            IsProbateSearch = probateLink != null;
            IsJusticeSearch = isCollinCounty && justiceLocation != null;
        }

        private static IWebElement? TryGetElement(IWebDriver driver, By selector, int retries, int wait)
        {
            var element = driver.TryFindElement(selector);
            while (element == null && retries > 0)
            {
                Thread.Sleep(wait);
                element = driver.TryFindElement(selector);
                retries--;
            }
            return element;
        }
    }
}