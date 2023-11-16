﻿using legallead.records.search.Dto;
using OpenQA.Selenium;

namespace legallead.records.search.Web
{
    using Byy = OpenQA.Selenium.By;

    public class ElementSendKeyAction : ElementActionBase
    {
        private const string actionName = "send-key";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            var driver = GetWeb;
            var selector = Byy.CssSelector(item.Locator.Query);
            var elementToClick = driver.FindElement(selector);
            if (string.IsNullOrEmpty(item.DisplayName))
            {
                return;
            }

            var objText = item.ExpectedValue;
            elementToClick.Click();
            var jse = (IJavaScriptExecutor)driver;
            jse.ExecuteScript("arguments[0].blur();", elementToClick);
        }
    }
}