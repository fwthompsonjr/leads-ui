using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace legallead.records.search.Classes
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor", "S6605:Collection-specific \"Exists\" method should be used instead of the \"Any\" extension", Justification = "<Pending>")]
    public class ElementAssertion
    {
        public ElementAssertion(IWebDriver driver)
        {
            PageDriver = driver;
        }

        public IWebDriver PageDriver { get; private set; }

        public bool DoesElementExist(By selector, string elementName)
        {
            Console.WriteLine(CommonKeyIndexes.SearchingForElement, elementName);
            PageDriver.FindElement(selector);
            Console.WriteLine(string.Format(
                CultureInfo.CurrentCulture,
                CommonKeyIndexes.ElementNotFound, elementName));
            return true;
        }

        public void SetSelectedIndex(By selector, string elementName, int selectedIndex)
        {
            string cmmd = CommonKeyIndexes.SetSelectElementIndex;
            Console.WriteLine(cmmd, elementName, selectedIndex);
            IWebElement elementToClick = PageDriver.FindElement(selector);
            string id = elementToClick.GetAttribute(CommonKeyIndexes.IdLowerCase);
            string command = string.Format(
                CultureInfo.CurrentCulture, CommonKeyIndexes.GetElementSetIndex,
                id, selectedIndex);
            string changecommand = string.Format(
                CultureInfo.CurrentCulture, CommonKeyIndexes.ElementFireOnChange,
                id);
            string optionName = string.Format(
                CultureInfo.CurrentCulture, CommonKeyIndexes.ElementGetOptionText,
                id, selectedIndex);

            IJavaScriptExecutor jse = (IJavaScriptExecutor)PageDriver;
            object rsp = jse.ExecuteScript(optionName);
            Console.WriteLine(CommonKeyIndexes.SetSelectOptionIndex, rsp.ToString());
            jse.ExecuteScript(command);
            jse.ExecuteScript(changecommand);
        }

        public void WaitForElementExist(By selector, string elementName, int secondsWait = 10)
        {
            Console.WriteLine(CommonKeyIndexes.WaitingForElement, elementName);
            try
            {
                WebDriverWait wait = new(PageDriver, TimeSpan.FromSeconds(secondsWait));
#pragma warning disable 618
                wait.Until(ExpectedConditions.ElementIsVisible(selector));
#pragma warning restore 618
            }
            catch (Exception)
            {
                // not gonna fire errors
            }
        }

        public void WaitForElementsToExist(By selector, string elementName, int secondsWait = 10)
        {
            Console.WriteLine(CommonKeyIndexes.WaitingForElement, elementName);
            try
            {
                WebDriverWait wait = new(PageDriver, TimeSpan.FromSeconds(secondsWait));
#pragma warning disable 618
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(selector));
#pragma warning restore 618
            }
            catch (Exception)
            {
                // not gonna fire errors
            }
        }

        public bool ContainsText(By selector, string elementName, string searchString)
        {
            if (!DoesElementExist(selector, elementName))
            {
                return false;
            }

            IWebElement found = PageDriver.FindElement(selector, 10);
            string message = string.Format(
                CultureInfo.CurrentCulture,
                CommonKeyIndexes.ElementTextNotFound, elementName, searchString);
            if (!found.Text.Contains(searchString))
            {
                Console.WriteLine(message);
                return false;
            }

            return true;
        }

        public bool MatchText(By selector, string elementName, string searchString)
        {
            if (!DoesElementExist(selector, elementName))
            {
                return false;
            }

            IWebElement found = PageDriver.FindElement(selector, 10);
            string message = string.Format(
                CultureInfo.CurrentCulture,
                CommonKeyIndexes.ElementMatchTextNotFound, elementName, searchString, found.Text);
            if (!found.Text.Equals(searchString, StringComparison.CurrentCulture))
            {
                Console.WriteLine(message);
                return false;
            }
            return true;
        }

        public bool ContainsClass(By selector, string elementName, string className)
        {
            if (!DoesElementExist(selector, elementName))
            {
                return false;
            }

            IWebElement found = PageDriver.FindElement(selector, 10);
            string classes = found.GetAttribute(CommonKeyIndexes.ClassAttribute) ?? string.Empty;
            string[] allClasses = classes.Split(' ');
            string message = string.Format(
                CultureInfo.CurrentCulture,
                CommonKeyIndexes.ElementClassNotFound, elementName, className);
            bool hasClass = allClasses.Any(x => x.Equals(className, StringComparison.CurrentCultureIgnoreCase));
            if (!hasClass)
            {
                Console.WriteLine(message);
            }
            return hasClass;
        }

        public bool QueryByClass(By selector, string elementName, string className)
        {
            if (!DoesElementExist(selector, elementName))
            {
                return false;
            }

            IWebElement found = PageDriver.FindElement(selector, 10);
            string classes = found.GetAttribute(CommonKeyIndexes.ClassAttribute) ?? string.Empty;
            string[] allClasses = classes.Split(' ');
            return allClasses.Any(x => x.Equals(className, StringComparison.CurrentCultureIgnoreCase));
        }

        public bool ContainsAttribute(By selector, string attributeName, string attributeValue)
        {
            IWebElement found = PageDriver.FindElement(selector, 10);
            string actual = found.GetAttribute(attributeName) ?? string.Empty;
            string message = string.Format(
                CultureInfo.CurrentCulture,
                CommonKeyIndexes.ElementAttributeNotFound,
                found.GetAttribute(CommonKeyIndexes.IdLowerCase),
                attributeName,
                attributeValue,
                actual);
            if (string.IsNullOrEmpty(attributeValue) && string.IsNullOrEmpty(actual))
            {
                return true;
            }

            bool hasAttribute = actual.Equals(attributeValue, StringComparison.CurrentCultureIgnoreCase);
            if (!hasAttribute)
            {
                Console.WriteLine(message);
            }
            return hasAttribute;
        }

        public bool DoesNotContainsClass(By selector, string elementName, string className)
        {
            if (!DoesElementExist(selector, elementName))
            {
                return false;
            }

            IWebElement found = PageDriver.FindElement(selector, 10);
            string classes = found.GetAttribute(CommonKeyIndexes.ClassAttribute) ?? string.Empty;
            string[] allClasses = classes.Split(' ');
            string message = string.Format(
                CultureInfo.CurrentCulture,
                CommonKeyIndexes.ClassNameFound, elementName, className);
            bool hasClass = allClasses.Any(x => x.Equals(className, StringComparison.CurrentCultureIgnoreCase));
            if (hasClass)
            {
                Console.WriteLine(message);
            }
            return !hasClass;
        }

        public void Navigate(string target)
        {
            Console.WriteLine(CommonKeyIndexes.NavigateToUrlMessage, target);
            Uri newUri = new(target);
            PageDriver.Navigate().GoToUrl(newUri);
        }

        /// <summary>
        /// Clicks the element.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="controlId">The control identifier.</param>
        public void ClickElement(string controlId)
        {
            try
            {
                Console.WriteLine(CommonKeyIndexes.ClickingOnElement, controlId);
                IJavaScriptExecutor jse = (IJavaScriptExecutor)PageDriver;
                jse.ExecuteScript(string.Format(
                CultureInfo.CurrentCulture,
                CommonKeyIndexes.ClickElementJs, controlId));
            }
            catch (Exception)
            {
                // no action as selenium is funny in this behavior
            }
        }

        /// <summary>
        /// Sets the text value.
        /// </summary>
        /// <param name="controlId">The control identifier.</param>
        /// <param name="controlValue">The control value.</param>
        public void ControlSetValue(string controlId, string controlValue)
        {
            try
            {
                controlId ??= string.Empty;

                controlValue ??= string.Empty;

                Console.WriteLine(CommonKeyIndexes.SettingControlValue, controlId,
                    controlId.Equals(CommonKeyIndexes.Password,
                        StringComparison.CurrentCultureIgnoreCase) ?
                        CommonKeyIndexes.PasswordMask : controlValue);
                IJavaScriptExecutor jse = (IJavaScriptExecutor)PageDriver;
                jse.ExecuteScript(string.Format(
                CultureInfo.CurrentCulture,
                CommonKeyIndexes.ControlSetValue, controlId, controlValue));
            }
            catch (Exception)
            {
                // no action as selenium is funny in this behavior
            }
        }

        /// <summary>
        /// Populates web forms with appropriate data as determined by the settings file.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        internal IWebElement? Process(WebInteractive data, bool isCriminalSearch = false)
        {
            string dteFmt = CommonKeyIndexes.DateTimeShort;
            string startDate = data.StartDate.ToString(dteFmt, CultureInfo.CurrentCulture.DateTimeFormat);
            string endDate = data.EndingDate.ToString(dteFmt, CultureInfo.CurrentCulture.DateTimeFormat);
            Models.WebNavigationKey? searchTypeIndex = data.Parameters.Keys
                .Find(x => x.Name.Equals(CommonKeyIndexes.SearchComboIndex,
                StringComparison.CurrentCultureIgnoreCase));
            int searchTypeId = searchTypeIndex == null ? 0 : Convert.ToInt32(searchTypeIndex.Value,
                CultureInfo.CurrentCulture);
            Models.WebNavigationKey? caseTypeIndex = data.Parameters.Keys
                .Find(x => x.Name.Equals(CommonKeyIndexes.CaseSearchType,
                StringComparison.CurrentCultureIgnoreCase));
            string caseType = caseTypeIndex == null ? string.Empty : caseTypeIndex.Value;
            Models.WebNavigationKey? districtSearchFlag = data.Parameters.Keys
                .Find(x => x.Name.Equals(CommonKeyIndexes.DistrictSearchType,
                StringComparison.CurrentCultureIgnoreCase));
            string districtType = districtSearchFlag == null ? string.Empty : districtSearchFlag.Value;
            bool isDistrictSearch = districtSearchFlag != null;
            List<Models.WebNavInstruction> itms = data.Parameters.Instructions;
            if (!isDistrictSearch)
            {
                itms.RemoveAll(x => x.FriendlyName.StartsWith(CommonKeyIndexes.DistrictDash, StringComparison.CurrentCultureIgnoreCase));
            }
            else if (isDistrictSearch & itms.Count < 15)
            {
                itms = SettingsManager.GetInstructions(1);
            }

            // substitute parameters
            itms.ForEach(x => x.Value = x.Value.Replace(CommonKeyIndexes.StartDateQuery, startDate));
            itms.ForEach(x => x.Value = x.Value.Replace(CommonKeyIndexes.EndingDateQuery, endDate));
            itms.ForEach(x => x.Value = x.Value.Replace(CommonKeyIndexes.SetComboIndexQuery,
                searchTypeId.ToString(CultureInfo.CurrentCulture)));
            if (!string.IsNullOrEmpty(caseType))
            {
                Models.WebNavigationKey? crimalLink = data.Parameters.Keys
                    .Find(x => x.Name.Equals(CommonKeyIndexes.CriminalLinkQuery,
                    StringComparison.CurrentCultureIgnoreCase));
                if (isCriminalSearch && crimalLink != null)
                {
                    caseType = crimalLink.Value;
                }
                List<Models.WebNavInstruction> caseSearchItems = itms.FindAll(x =>
                    x.FriendlyName.Equals(CommonKeyIndexes.SearchHyperlink,
                        StringComparison.CurrentCultureIgnoreCase));

                caseSearchItems.ForEach(x => x.Value = caseType);
            }
            if (!string.IsNullOrEmpty(districtType))
            {
                List<Models.WebNavInstruction> districtItems = itms.FindAll(x =>
                    x.FriendlyName.Equals(CommonKeyIndexes.DistrictHyperlink,
                        StringComparison.CurrentCultureIgnoreCase));
                districtItems.ForEach(x => x.Value = districtType);
            }
            List<ElementNavigationBase> navigations = GetNavigationBases(startDate);

            foreach (Models.WebNavInstruction item in itms)
            {
                Console.WriteLine(
                    CommonKeyIndexes.WebNavInstructionMessage,
                    item.Name,
                    item.FriendlyName,
                    item.Value);
                ElementNavigationBase? navigator = navigations.Find(f =>
                    f.Name.Equals(item.Name, StringComparison.CurrentCultureIgnoreCase));
                if (navigator == null)
                {
                    continue;
                }

                IWebElement? webElement = navigator.Execute(item);
                if (webElement != null)
                {
                    return webElement;
                }
            }
            return null;
        }

        protected List<ElementNavigationBase> GetNavigationBases(string startDate)
        {
            List<ElementNavigationBase> list = ElementNavigations;
            list.ForEach(x =>
            {
                x.StartDate = startDate;
                x.Assertion = this;
            });
            return list;
        }

        private static List<ElementNavigationBase>? _navigationElements;

        private static List<ElementNavigationBase> ElementNavigations => _navigationElements ??= GetNavigators();

        private static List<ElementNavigationBase> GetNavigators()
        {
            Type type = typeof(ElementNavigationBase);
            List<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                .ToList();
            List<ElementNavigationBase> commands = new();
            types.ForEach(f =>
            {
                var item = GetInstance(f);
                if (item != null) commands.Add(item);
            });
            return commands;
        }

        private static ElementNavigationBase? GetInstance(Type type)
        {
            var obj = Activator.CreateInstance(type);
            if (obj is not ElementNavigationBase nav) return null;
            return nav;
        }
    }
}