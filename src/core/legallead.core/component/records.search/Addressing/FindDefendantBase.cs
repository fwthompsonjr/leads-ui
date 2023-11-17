using legallead.records.search.Models;
using OpenQA.Selenium;
using System.Text;

namespace legallead.records.search.Addressing
{
    public abstract class FindDefendantBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance can find.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can find; otherwise, <c>false</c>.
        /// </value>
        public abstract bool CanFind { get; set; }

        /// <summary>
        /// Finds the specified defendant record from web page source.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="webInteractive">The web interactive.</param>
        /// <param name="linkData">The link data.</param>
        public abstract void Find(IWebDriver driver, HLinkDataRow linkData);

        /// <summary>
        /// Locates the address element from the case-detail drill down page.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="trCol">The tr col.</param>
        /// <returns></returns>
        protected static IWebElement GetAddressRow(IWebElement parent, System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> trCol)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            if (trCol == null)
            {
                throw new ArgumentNullException(nameof(trCol));
            }

            int colIndex = 3;
            parent = trCol[colIndex];
            string parentTxt = string.IsNullOrEmpty(parent.Text) ? string.Empty : parent.Text;
            if (string.IsNullOrEmpty(parentTxt))
            {
                parent = trCol[colIndex - 1];
            }

            return parent;
        }

        /// <summary>
        /// Tries the find element on a specfic web page using the By condition supplied.
        /// </summary>
        /// <param name="parent">The parent web browser instance.</param>
        /// <param name="by">The by condition used to locate the element</param>
        /// <returns></returns>
        protected static IWebElement? TryFindElement(IWebDriver parent, By by)
        {
            try
            {
                if (parent == null)
                {
                    return null;
                }

                return parent.FindElement(by);
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected static string GetAddress(List<IWebElement> elements, string keyword = "defendant")
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            if (string.IsNullOrEmpty(keyword)) { keyword = "defendant"; }
            const StringComparison currentCase = StringComparison.CurrentCultureIgnoreCase;
            CultureInfo cultureInfo = System.Globalization.CultureInfo.CurrentCulture;
            string address = string.Empty;
            string addressHtml = string.Empty;
            string findKey = keyword.ToLower(cultureInfo);
            if (!elements.Any())
            {
                return string.Empty;
            }

            int rowIndex = 0;
            while (address.ToLower(cultureInfo).StartsWith(findKey, currentCase) | string.IsNullOrEmpty(address))
            {
                if (rowIndex > elements.Count - 1)
                {
                    break;
                }

                IWebElement currentRow = elements[rowIndex];
                List<IWebElement> cells = currentRow.FindElements(By.TagName(IndexKeyNames.TdElement)).ToList();
                IWebElement? firstTd = cells.Any() ? cells[0] : null;
                addressHtml = firstTd != null ? firstTd.GetAttribute("innerHTML").Trim() : string.Empty;
                address = currentRow.GetAttribute("innerText")
                    .Replace(Environment.NewLine, CommonKeyIndexes.BrElementTag).Trim();
                rowIndex += 1;
                System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> headings = currentRow.FindElements(By.TagName(IndexKeyNames.ThElement));

                if (headings != null && headings.Count > 1 && !address.ToLower(cultureInfo).StartsWith(findKey, currentCase))
                {
                    address = string.Empty;
                    break;
                }
                if (firstTd != null &&
                    string.IsNullOrEmpty(firstTd.GetAttribute("headers")))
                {
                    address = string.Empty;
                }
            }
            if (string.IsNullOrEmpty(addressHtml))
            {
                return address;
            }
            // custom clean ups for collin-county
            const string noBr = @"<nobr>";
            string br = CommonKeyIndexes.BrElementTag;
            if (addressHtml.IndexOf(noBr, currentCase) < 0)
            {
                return address;
            }
            int endOfLine = addressHtml.IndexOf(noBr, currentCase);
            addressHtml = addressHtml[..endOfLine];
            addressHtml = new StringBuilder(addressHtml)
                .Replace("&nbsp;", " ")
                .Replace("<br>", br)
                .Replace(Environment.NewLine, br)
                .Replace(br, "|").ToString();
            return addressHtml;
        }

        protected static void MapElementAddress(HLinkDataRow linkData,
            IWebElement rowLabel,
            IWebElement table,
            List<IWebElement> trCol,
            int r,
            string searchTitle)
        {
            if (linkData == null)
            {
                throw new ArgumentNullException(nameof(linkData));
            }

            if (rowLabel == null)
            {
                throw new ArgumentNullException(nameof(rowLabel));
            }

            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            if (trCol == null)
            {
                throw new ArgumentNullException(nameof(trCol));
            }

            IWebElement? nextTh = table.FindElements(By.TagName(IndexKeyNames.ThElement)).FirstOrDefault(x => x.Location.Y > rowLabel.Location.Y);
            int mxRowIndex = nextTh == null ? r : Convert.ToInt32(nextTh.FindElement(By.XPath(IndexKeyNames.ParentElement)).GetAttribute(IndexKeyNames.RowIndex),
                CultureInfo.CurrentCulture.NumberFormat);
            while (r <= mxRowIndex)
            {
                IWebElement currentRow = trCol[r];
                List<IWebElement> tdElements = currentRow.FindElements(By.TagName(IndexKeyNames.TdElement)).ToList();
                tdElements = tdElements.FindAll(x => x.Location.X >= rowLabel.Location.X & x.Location.X < (rowLabel.Location.X + rowLabel.Size.Width));
                linkData.Address = GetAddress(tdElements, searchTitle);
                if (!string.IsNullOrEmpty(linkData.Address))
                {
                    break;
                }

                r += 1;
            }
            linkData.Address = NoFoundMatch.GetNoMatch(linkData.Address);
        }

        protected static class IndexKeyNames
        {
            public static readonly string RowIndex = ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.RowIndex);
            public static readonly string ThContainsText = ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.ThContainsText);
            public static readonly string ParentElement = ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.ParentElement);
            public static readonly string ThElement = ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.ThElement);
            public static readonly string InnerText = ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.InnerText);
            public static readonly string TrElement = ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.TrElement);
            public static readonly string Applicant = ResourceTable.GetText(ResourceType.FindDefendant, ResourceKeyIndex.Applicant);

            public static readonly string Defendant
                = ResourceTable.GetText(ResourceType.FindDefendant, ResourceKeyIndex.Defendant);

            internal static string TdElement
                = ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.TdElement);
        }
    }
}