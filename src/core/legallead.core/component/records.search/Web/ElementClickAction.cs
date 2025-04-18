﻿using legallead.records.search.Classes;
using legallead.records.search.Dto;
using OpenQA.Selenium;

namespace legallead.records.search.Web
{
    public class ElementClickAction : ElementActionBase
    {
        private const string actionName = "click";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            int retries = 5;
            IWebDriver? driver = GetWeb;
            By? selector = GetSelector(item);
            if (driver == null || selector == null) { return; }
            IWebElement? elementToClick = driver.TryFindElement(selector);
            while (elementToClick == null && retries > 0)
            {
                Thread.Sleep(500);
                elementToClick = driver.TryFindElement(selector);
                retries--;
            }
            Console.WriteLine("Element click action -- : " + selector);
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("arguments[0].click();", elementToClick);
            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }
    }
}