using legallead.records.search.Dto;

namespace legallead.records.search.Web
{
    using Byy = OpenQA.Selenium.By;

    public class ElementExistsAction : ElementActionBase
    {
        private const string actionName = "exists";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            var selector = Byy.CssSelector(item.Locator.Query);
            GetAssertion.WaitForElementExist(
                selector,
                string.Format("Looking for {0}", item.DisplayName));
        }
    }
}