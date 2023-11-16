namespace legallead.records.search.Web
{
    using legallead.records.search.Dto;
    using OpenQA.Selenium;
    using System.Threading;

    public class JquerySetSelectedIndex : ElementActionBase
    {
        private const string actionName = "jquery-set-selected-index";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            IWebDriver driver = GetWeb;
            string selector = item.Locator.Query;
            if (string.IsNullOrEmpty(selector))
            {
                return;
            }

            string objText = item.ExpectedValue;
            string command = $"$('{selector}').prop('selectedIndex', {objText});";

            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
            jse.ExecuteScript(command);

            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }
    }
}