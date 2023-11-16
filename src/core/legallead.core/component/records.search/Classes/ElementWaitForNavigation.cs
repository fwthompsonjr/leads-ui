using legallead.records.search.Models;
using OpenQA.Selenium;

namespace legallead.records.search.Classes
{
    public class ElementWaitForNavigation : ElementNavigationBase
    {
        public override IWebElement Execute(WebNavInstruction item)
        {
            if (Assertion == null)
            {
                return null;
            }

            if (PageDriver == null)
            {
                return null;
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            PageDriver.WaitForNavigation();
            return null;
        }
    }
}