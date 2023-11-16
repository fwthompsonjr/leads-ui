namespace legallead.records.search.Web
{
    using legallead.records.search.Dto;
    using Byy = OpenQA.Selenium.By;

    public class ElementTextConfirmAction : ElementActionBase
    {
        private const string actionName = "text-confirm";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            var selector = Byy.CssSelector(item.Locator.Query);
            GetAssertion.MatchText(selector,
                item.DisplayName, item.ExpectedValue);
        }
    }
}