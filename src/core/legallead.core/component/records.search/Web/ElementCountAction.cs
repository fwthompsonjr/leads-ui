﻿using legallead.records.search.Dto;

namespace legallead.records.search.Web
{
    using Byy = OpenQA.Selenium.By;

    public class ElementCountAction : ElementActionBase
    {
        private const string actionName = "count";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            OpenQA.Selenium.IWebDriver? driver = GetWeb;
            if (driver == null) { return; }
            Byy selector = Byy.CssSelector(item.Locator.Query);
            if (!int.TryParse(item.ExpectedValue, out int number))
            {
                return;
            }

            System.Collections.ObjectModel.ReadOnlyCollection<OpenQA.Selenium.IWebElement> matches = driver.FindElements(selector) ?? throw new ArgumentOutOfRangeException(item.DisplayName,
                    string.Format(
                    "Expected element collection {0} not found",
                    item.DisplayName));
            if (matches.Count != number)
            {
                throw new ArgumentOutOfRangeException(item.DisplayName,
                    string.Format(
                    "Expected element count {0} mismatch. Expected {1}, Actual {2}",
                    item.DisplayName,
                    number,
                    matches.Count));
            }
            else
            {
                Console.WriteLine("Expected element count {0} matched. Expected {1}, Actual {2}",
                    item.DisplayName,
                    number,
                    matches.Count);
            }
        }
    }
}