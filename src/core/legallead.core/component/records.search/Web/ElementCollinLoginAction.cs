using legallead.records.search.Classes;
using legallead.records.search.Dto;
using OpenQA.Selenium;
using System.Text;

namespace legallead.records.search.Web
{
    using Byy = OpenQA.Selenium.By;

    public class ElementCollinLoginAction : ElementActionBase
    {
        private const string actionName = "login-collin-county";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            IWebDriver driver = GetWeb;
            UserAccessDto userDto = UserAccessDto.GetDto(item.ExpectedValue);
            string pwordUser = CryptoEngine.Decrypt(userDto.UserGuid, userDto.UserKey, userDto.UserData);
            string[] userId = pwordUser.Split('|');
            string[] selections = item.Locator.Query.Split('|');
            StringBuilder script = new();
            string line = Environment.NewLine;
            script.AppendFormat(CultureInfo.CurrentCulture, "document.getElementById('UserName').value = '{0}'{1}", userId[0], line);
            script.AppendFormat(CultureInfo.CurrentCulture, "document.getElementById('Password').value = '{0}'{1}", userId[1], line);

            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript(script.ToString());
            Thread.Sleep(500);

            foreach (string itm in selections)
            {
                Byy selector = Byy.CssSelector(itm.Trim());
                IWebElement elementToClick = driver.FindElement(selector);
                executor.ExecuteScript("arguments[0].focus();", elementToClick);
                Thread.Sleep(300);
                executor.ExecuteScript("arguments[0].blur();", elementToClick);
            }
            executor.ExecuteScript("document.getElementById('Login').submit()");
            driver.WaitForNavigation();
        }
    }
}