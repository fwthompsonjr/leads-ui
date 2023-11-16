using legallead.records.search.Models;
using OpenQA.Selenium;

namespace legallead.records.search.Addressing
{
    public class FindDefendantByWordMatch : FindDefendantBase
    {
        public override bool CanFind { get; set; }

        public override void Find(IWebDriver driver, HLinkDataRow linkData)
        {
            // driver.FindElement(By.XPath("//th[contains(text(),'Principal')]"))
            // todo: CV-2019-02188-JP... exmple of a case with multiple defendants...
            // create a lookup for multiples
            if (driver == null)
            {
                throw new System.ArgumentNullException(nameof(driver));
            }

            if (linkData == null)
            {
                throw new System.ArgumentNullException(nameof(linkData));
            }

            CanFind = false;
            string defendant = IndexKeyNames.Defendant;
            string defendantXpath = string.Format(CultureInfo.CurrentCulture,
                IndexKeyNames.ThContainsText, defendant);
            IWebElement tdName = TryFindElement(driver, By.XPath(defendantXpath));
            // this instance can find
            if (tdName == null)
            {
                return;
            }

            IWebElement parent = tdName.FindElement(By.XPath(IndexKeyNames.ParentElement));
            IWebElement rowLabel = parent.FindElements(By.TagName(IndexKeyNames.ThElement))[1];
            linkData.Defendant = rowLabel.GetAttribute(IndexKeyNames.InnerText);
            CanFind = true;
            linkData.Address = parent.Text;
            try
            {
                // get row index of this element ... and then go one row beyond...
                string ridx = parent.GetAttribute(IndexKeyNames.RowIndex);
                IWebElement table = parent.FindElement(By.XPath(IndexKeyNames.ParentElement));
                List<IWebElement> trCol = table.FindElements(By.TagName(IndexKeyNames.TrElement)).ToList();
                if (!int.TryParse(ridx, out int r))
                {
                    return;
                }

                IWebElement? nextTh = table.FindElements(By.TagName(IndexKeyNames.ThElement)).FirstOrDefault(x => x.Location.Y > rowLabel.Location.Y);
                int mxRowIndex = nextTh == null ? r + 1 :
                    Convert.ToInt32(nextTh.FindElement(By.XPath(IndexKeyNames.ParentElement)).GetAttribute(IndexKeyNames.RowIndex),
                    CultureInfo.CurrentCulture.NumberFormat);
                while (r <= mxRowIndex)
                {
                    IWebElement currentRow = trCol[r];
                    List<IWebElement> tdElements = currentRow.FindElements(By.TagName(IndexKeyNames.TdElement)).ToList();
                    tdElements = tdElements.FindAll(x => x.Location.X >= rowLabel.Location.X & x.Location.X < (rowLabel.Location.X + rowLabel.Size.Width));
                    linkData.Address = GetAddress(tdElements);
                    if (!string.IsNullOrEmpty(linkData.Address))
                    {
                        break;
                    }

                    r += 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}