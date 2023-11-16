using legallead.records.search.Models;
using OpenQA.Selenium;

namespace legallead.records.search.Classes
{
    public class ElementSetControlValue : ElementNavigationBase
    {
        public override IWebElement Execute(WebNavInstruction item)
        {
            if (Assertion == null)
            {
                return null;
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            string[] indexes = item.Value.Split(comma);
            string idx = indexes[0];
            string txt = indexes[1];
            if (item.FriendlyName.Equals(CommonKeyIndexes.DateFiledOnTextBox,
                StringComparison.CurrentCultureIgnoreCase))
            {
                txt = StartDate;
            }
            Assertion.ControlSetValue(idx, txt);
            return null;
            throw new NotImplementedException();
        }
    }
}