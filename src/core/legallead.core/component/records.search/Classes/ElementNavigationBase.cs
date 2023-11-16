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
                string typeName = GetType().Name;
                int startAt = element.Length;
                return typeName[startAt..];
            }
        }

        public string StartDate { get; set; }

        public ElementAssertion Assertion { get; set; }

        public abstract IWebElement Execute(WebNavInstruction item);

        protected IWebDriver PageDriver => Assertion?.PageDriver;
    }
}