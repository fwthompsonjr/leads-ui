namespace legallead.records.search.Web
{
    using legallead.records.search.Dto;
    using OpenQA.Selenium;
    using System.Threading;

    public class JquerySetSelectedValue : ElementActionBase
    {
        private const string actionName = "jquery-set-selected-value";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            IWebDriver? driver = GetWeb;
            if (driver == null) { return; }
            string selector = item.Locator.Query;
            if (string.IsNullOrEmpty(selector))
            {
                return;
            }

            string objText = item.ExpectedValue;
            string command = $"$('{selector}').val('{objText}'); $('{selector}').trigger('change');";

            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
            jse.ExecuteScript(command);

            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }
    }
}