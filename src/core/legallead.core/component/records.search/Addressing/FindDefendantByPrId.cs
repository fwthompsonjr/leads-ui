using legallead.records.search.Models;
using OpenQA.Selenium;
using System.Text;

namespace legallead.records.search.Addressing
{
    public class FindDefendantByPrId : FindDefendantBase
    {
        public override bool CanFind { get; set; }

        public override void Find(IWebDriver driver, HLinkDataRow linkData)
        {
            if (driver == null)
            {
                throw new System.ArgumentNullException(nameof(driver));
            }

            if (linkData == null)
            {
                throw new System.ArgumentNullException(nameof(linkData));
            }

            CanFind = false;
            IWebElement? tdName = TryFindElement(driver, By.XPath(@"//*[@id='PIr11']"));
            // this instance can find
            if (tdName == null)
            {
                return;
            }

            linkData.Defendant = tdName.GetAttribute("innerText");
            IWebElement parent = tdName.FindElement(By.XPath(IndexKeyNames.ParentElement));
            IWebElement rowLabel = parent.FindElements(By.TagName(IndexKeyNames.ThElement))[0];
            if (rowLabel.Text.Trim()
                .ToLower(System.Globalization.CultureInfo.CurrentCulture) != "defendant")
            {
                return;
            }
            CanFind = true;
            linkData.Address = parent.Text;
            try
            {
                // get row index of this element ... and then go one row beyond...
                string ridx = parent.GetAttribute(IndexKeyNames.RowIndex);
                IWebElement table = parent.FindElement(By.XPath(IndexKeyNames.ParentElement));
                System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> trCol = table.FindElements(By.TagName(IndexKeyNames.TrElement));
                if (!int.TryParse(ridx, out int r))
                {
                    return;
                }

                parent = GetAddressRow(parent, trCol); // put this row-index into config... it can change
                linkData.Address = new StringBuilder(parent.Text).Replace(Environment.NewLine, "<br/>").ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}