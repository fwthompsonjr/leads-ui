﻿// FindRespondentByWordMatch
using legallead.records.search.Models;
using OpenQA.Selenium;

namespace legallead.records.search.Addressing
{
    public class FindRespondentByWordMatch : FindDefendantBase
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

            string searchType = "Respondent";
            CanFind = false;
            IWebElement? tdName = TryFindElement(driver, By.XPath(
                String.Format(IndexKeyNames.ThContainsText, searchType)));
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

                MapElementAddress(linkData, rowLabel, table, trCol, r, searchType.ToLower(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}