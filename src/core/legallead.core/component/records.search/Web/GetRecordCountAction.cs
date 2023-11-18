using legallead.records.search.Classes;
using legallead.records.search.Dto;

namespace legallead.records.search.Web
{
    public class GetRecordCountAction : ElementActionBase
    {
        private const string actionName = "get-record-count";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            OpenQA.Selenium.By? selector = GetSelector(item);
            if (selector == null) return;
            OpenQA.Selenium.IWebElement? element = GetWeb?.TryFindElement(selector);
            if (element != null)
            {
                Console.WriteLine("Search found {0} records.", element.Text);
            }
        }
    }
}