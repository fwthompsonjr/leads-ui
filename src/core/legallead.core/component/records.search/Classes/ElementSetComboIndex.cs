using legallead.records.search.Models;
using OpenQA.Selenium;

namespace legallead.records.search.Classes
{
    public class ElementSetComboIndex : ElementNavigationBase
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
            string[] parms = item.Value.Split(comma);
            string parmId = parms[0];
            if (!int.TryParse(parms[1], out int parmIndex))
            {
                parmIndex = 0;
            }
            Assertion.SetSelectedIndex(By.Id(parmId), item.FriendlyName, parmIndex);
            return null;
        }
    }
}