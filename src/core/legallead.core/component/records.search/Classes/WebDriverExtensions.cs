﻿using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace legallead.records.search.Classes
{
    public static class WebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            if (timeoutInSeconds <= 0)
            {
                return driver.FindElement(by);
            }

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(drv => drv.FindElement(by).Displayed);
            return driver.FindElement(by);
        }

        public static void WaitForNavigation(this IWebDriver driver)
        {
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(30));

            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public static void ClickAndOrSetText(this IWebDriver driver, IWebElement elementToClick, string objText = "")
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            if (elementToClick == null)
            {
                throw new ArgumentNullException(nameof(elementToClick));
            }

            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("arguments[0].click();", elementToClick);
            if (!string.IsNullOrEmpty(objText))
            {
                string script = string.Format(CultureInfo.InvariantCulture, "arguments[0].value = '{0}';", objText);
                executor.ExecuteScript(script, elementToClick);
            }
        }

        public static void ClearText(this IWebDriver driver, IWebElement elementToClick)
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            if (elementToClick == null)
            {
                throw new ArgumentNullException(nameof(elementToClick));
            }

            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("arguments[0].value = '';", elementToClick);
        }

        public static void SetText(this IWebDriver driver, IWebElement elementToClick, string objText)
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            if (elementToClick == null)
            {
                throw new ArgumentNullException(nameof(elementToClick));
            }

            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            string script = string.Format(CultureInfo.InvariantCulture, "arguments[0].value = '{0}';", objText);
            executor.ExecuteScript(script, elementToClick);
        }

        public static bool IsElementPresent(this IWebDriver driver, By by)
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            try
            {
                ElementAssertion assertion = new(driver);
                assertion.WaitForElementExist(by, "Element");
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static bool AreElementsPresent(this IWebDriver driver, By by)
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            try
            {
                ElementAssertion assertion = new(driver);
                assertion.WaitForElementsToExist(by, "ElementCollection", 5);
                driver.FindElements(by);
                return true;
            }
            catch (Exception)
            {
                // intentional to catch all exceptions
                return false;
            }
        }
    }
}