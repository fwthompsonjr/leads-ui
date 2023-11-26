using legallead.records.search.Classes;
using legallead.records.search.Dto;
using OpenQA.Selenium;

namespace legallead.records.search.Web
{
    using Byy = OpenQA.Selenium.By;

    public class ElementSetPasswordAction : ElementActionBase
    {
        private const string actionName = "login";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            IWebDriver? driver = GetWeb;
            if (driver == null) return;
            UserAccessDto userDto = UserAccessDto.GetDto(item.ExpectedValue) ?? new();
            string pwordUser = CryptoEngine.Decrypt(userDto.UserGuid, userDto.UserKey, userDto.UserData);
            string[] userId = pwordUser.Split('|');
            string[] selections = item.Locator.Query.Split('|');
            int idx = 0;
            foreach (string itm in selections)
            {
                Byy selector = Byy.CssSelector(itm.Trim());
                IWebElement elementToClick = driver.FindElement(selector);
                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                string command = $"arguments[0].value = '{userId[idx]}'";
                executor.ExecuteScript("arguments[0].focus();", elementToClick);
                executor.ExecuteScript(command, elementToClick);
                executor.ExecuteScript("arguments[0].blur();", elementToClick);
                idx++;
            }
            driver.WaitForNavigation();
        }
    }
}