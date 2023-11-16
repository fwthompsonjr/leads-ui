﻿// FindMultipleDefendantMatch
using legallead.records.search.Models;
using OpenQA.Selenium;

namespace legallead.records.search.Addressing
{
    public class FindMultipleDefendantMatch : FindDefendantBase
    {
        public override bool CanFind { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types",
            Justification = "Exception thrown from this method will stop automation.")]
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
            // driver.FindElement(By.XPath("//th[contains(text(),'Principal')]"))
            // todo: CV-2019-02188-JP... exmple of a case with multiple defendants...
            // create a lookup for multiples
            const string xpath = @"//th[contains(text(),'Defendant')]";
            CanFind = false;
            var tdName = TryFindElement(driver, By.XPath(xpath));
            // this instance can find
            if (tdName == null)
            {
                return;
            }

            var tdCollection = driver.FindElements(By.XPath(xpath));
            if (tdCollection.Count <= 1)
            {
                return;
            }

            foreach (var tdItem in tdCollection)
            {
                var parent = tdItem.FindElement(By.XPath(IndexKeyNames.ParentElement));
                var rowLabel = parent.FindElements(By.TagName(IndexKeyNames.ThElement))[1];
                linkData.Defendant = rowLabel.GetAttribute(IndexKeyNames.InnerText);
                CanFind = true;
                linkData.Address = parent.Text;
                try
                {
                    // get row index of this element ... and then go one row beyond...
                    var ridx = parent.GetAttribute(IndexKeyNames.RowIndex);
                    var table = parent.FindElement(By.XPath(IndexKeyNames.ParentElement));
                    var trCol = table.FindElements(By.TagName(IndexKeyNames.TrElement)).ToList();
                    if (!int.TryParse(ridx, out int r))
                    {
                        return;
                    }

                    var nextTh = table.FindElements(By.TagName(IndexKeyNames.ThElement)).ToList().FirstOrDefault(x => x.Location.Y > rowLabel.Location.Y);
                    var mxRowIndex = nextTh == null ? r :
                        Convert.ToInt32(
                            nextTh.FindElement(By.XPath(IndexKeyNames.ParentElement)).GetAttribute(IndexKeyNames.RowIndex),
                            CultureInfo.CurrentCulture.NumberFormat);
                    while (r <= mxRowIndex)
                    {
                        var currentRow = trCol[r];
                        var tdElements = currentRow.FindElements(By.TagName(IndexKeyNames.TdElement)).ToList();
                        tdElements = tdElements.FindAll(x => x.Location.X >= rowLabel.Location.X & x.Location.X < (rowLabel.Location.X + rowLabel.Size.Width));
                        linkData.Address = GetAddress(tdElements);
                        if (!string.IsNullOrEmpty(linkData.Address))
                        {
                            break;
                        }

                        r += 1;
                    }
                    if (!string.IsNullOrEmpty(linkData.Address))
                    {
                        return;
                    }
                }
                catch (Exception)
                {
                }
            }
            linkData.Address = NoFoundMatch.GetNoMatch(linkData.Address);
        }
    }
}