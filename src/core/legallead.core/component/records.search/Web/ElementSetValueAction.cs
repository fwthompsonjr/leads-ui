﻿using legallead.records.search.Dto;
using OpenQA.Selenium;

namespace legallead.records.search.Web
{
    using Byy = OpenQA.Selenium.By;

    public class ElementSetValueAction : ElementActionBase
    {
        private const string actionName = "set-text";

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
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("arguments[0].click();", elementToClick);
            Thread.Sleep(350);
            elementToClick.SendKeys(objText);
            executor.ExecuteScript(string.Format("arguments[0].value = '{0}';", objText), elementToClick);
            Thread.Sleep(350);
            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }
    }
}