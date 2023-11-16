using legallead.records.search.Dto;

namespace legallead.records.search.Web
{
    public class WebNavAction : ElementActionBase
    {
        private const string actionName = "navigate";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            OpenQA.Selenium.IWebDriver driver = GetWeb;
            Uri uri = new(item.Locator.Query);
            driver.Navigate().GoToUrl(uri);
            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }
    }
}