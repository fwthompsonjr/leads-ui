using legallead.records.search.Classes;
using legallead.records.search.Dto;
using OpenQA.Selenium;

// using legallead.records.search.Classes.WebElementExtensions;
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
            CollinWebInteractive helper = new();
            By selector = GetSelector(item);
            IWebElement element = GetWeb.FindElement(selector);
            string outerHtml = element.GetAttribute("outerHTML");
            outerHtml = helper.RemoveElement(outerHtml, "<img");
            // remove colspan? <colgroup>
            outerHtml = helper.RemoveTag(outerHtml, "colgroup");
            // remove the image tags now

            OuterHtml = outerHtml;
            string probateLinkXpath = CommonKeyIndexes.ProbateLinkXpath;
            string justiceLinkXpath = probateLinkXpath.Replace("'Probate'", "'Justice'");
            IWebElement probateLink =
                GetWeb.TryFindElement(
                    By.XPath(probateLinkXpath));
            IWebElement justiceLocation =
                GetWeb.TryFindElement(
                    By.XPath(justiceLinkXpath));
            bool isCollinCounty = GetWeb.Url.Contains("co.collin.tx.us");

            IsProbateSearch = probateLink != null;
            IsJusticeSearch = isCollinCounty && justiceLocation != null;
        }
    }
}