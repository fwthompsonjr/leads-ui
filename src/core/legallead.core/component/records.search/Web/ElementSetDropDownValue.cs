// ElementSetDropDownValue
namespace legallead.records.search.Web
{
    using legallead.records.search.Dto;
    using System.Threading;
    using Byy = OpenQA.Selenium.By;
    using DrpDwn = OpenQA.Selenium.Support.UI.SelectElement;

    public class ElementSetDropDownValue : ElementActionBase
    {
        private const string actionName = "set-dropdown-value";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            OpenQA.Selenium.IWebDriver driver = GetWeb;
            Byy selector = Byy.CssSelector(item.Locator.Query);
            OpenQA.Selenium.IWebElement elementToClick = driver.FindElement(selector);
            // lets get this item as a SELECT
            DrpDwn dropDown = new(elementToClick);
            if (string.IsNullOrEmpty(item.DisplayName))
            {
                return;
            }

            string objText = item.ExpectedValue;
            int mxIndex = dropDown.Options.Count - 1;
            int selectedIndex = System.Convert.ToInt32(objText);
            if (selectedIndex > mxIndex)
            {
                selectedIndex = mxIndex;
            }

            dropDown.SelectByIndex(selectedIndex);

            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }
    }
}