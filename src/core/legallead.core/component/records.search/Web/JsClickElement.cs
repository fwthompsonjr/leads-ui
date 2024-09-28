using legallead.records.search.Classes;
using legallead.records.search.Dto;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics.CodeAnalysis;

namespace legallead.records.search.Web
{
    public class JsClickElement : ElementActionBase
    {
        private const string actionName = "harris-civil-search-click";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            IWebDriver? driver = GetWeb;
            By? selector = GetSelector(item);
            if (driver == null || selector == null) { return; }
            IWebElement? elementToClick = driver.TryFindElement(selector);
            elementToClick ??= WaitForElementFound(driver, selector);
            Console.WriteLine("Element click action -- : " + selector);
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("arguments[0].click();", elementToClick);
            Console.WriteLine("-- begin: wait for navigation --");
            WaitForNextPage(driver);
            Console.WriteLine("-- ending: wait for navigation --");
        }
        [ExcludeFromCodeCoverage]
        private static void WaitForNextPage(IWebDriver driver)
        {
            const string countLabel = "ctl00_ContentPlaceHolder1_lblCount";
            By selector = By.Id(countLabel);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15))
            {
                PollingInterval = TimeSpan.FromMilliseconds(300),
            };
            wait.IgnoreExceptionTypes(typeof(ElementNotInteractableException));

            wait.Until(d =>
            {
                try
                {
                    var isfound = d.TryFindElement(selector) != null;
                    return isfound;
                }
                catch { return false; }
            });
        }
        [ExcludeFromCodeCoverage]
        private static IWebElement? WaitForElementFound(IWebDriver driver, By selector)
        {
            int retries = 5;
            while (retries > 0)
            {
                var elementToClick = driver.TryFindElement(selector);
                if (elementToClick != null) { return elementToClick; }
                retries--;
            }
            return null;
        }
    }
}