using legallead.records.search.Models;
using OpenQA.Selenium;

namespace legallead.records.search.Classes
{
    public abstract class ElementNavigationBase
    {
        private const string element = "Element";

        protected const char comma = ',';

        public virtual string Name
        {
            get
            {
                var typeName = GetType().Name;
                var startAt = element.Length;
                return typeName.Substring(startAt);
            }
        }

        public string StartDate { get; set; }

        public ElementAssertion Assertion { get; set; }

        public abstract IWebElement Execute(WebNavInstruction item);

        protected IWebDriver PageDriver => Assertion?.PageDriver;
    }
}