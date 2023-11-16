using legallead.records.search.Classes;
using legallead.records.search.Dto;
using legallead.records.search.Interfaces;
using legallead.records.search.Models;
using OpenQA.Selenium;

namespace legallead.records.search.Web
{
    public abstract class ElementActionBase : IElementActionBase
    {
        public string OuterHtml { get; set; }

        public virtual ElementAssertion GetAssertion { get; set; }

        public virtual IWebDriver GetWeb { get; set; }

        public virtual string ActionName { get; }

        public abstract void Act(NavigationStep item);

        public WebNavigationParameter GetSettings(int index)
        {
            var websites = SettingsManager.GetNavigation();

            var siteData = websites.First(x => x.Id == index);
            return siteData;
        }

        protected static By GetSelector(NavigationStep item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            const System.StringComparison comparison = System.StringComparison.CurrentCultureIgnoreCase;
            if (item.Locator.Find.Equals("css", comparison))
            {
                return By.CssSelector(item.Locator.Query);
            }

            if (item.Locator.Find.Equals("xpath", comparison))
            {
                return By.XPath(item.Locator.Query);
            }

            return null;
        }
    }
}