namespace legallead.records.search.Web
{
    using legallead.records.search.Dto;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using System.Threading;

    public class JquerySetSelectedIndex : ElementActionBase
    {
        private const string actionName = "jquery-set-selected-index";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            IWebDriver? driver = GetWeb;
            if (driver == null) { return; }
            WaitForLoad(driver);
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
        protected static void WaitForLoad(IWebDriver driver, int timeoutSec = 15)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, timeoutSec));
            wait.Until(wd => js.ExecuteScript("return document.readyState").ToString() == "complete");
        }
    }
}