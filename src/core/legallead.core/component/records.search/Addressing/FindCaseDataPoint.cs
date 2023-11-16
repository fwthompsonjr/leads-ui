﻿using legallead.records.search.Dto;
using legallead.records.search.Models;
using OpenQA.Selenium;

namespace legallead.records.search.Addressing
{
    public class FindCaseDataPoint : FindDefendantBase
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

            CanFind = false;
            var dto = DataPointLocatorDto.GetDto("tarrantCountyDataPoint");
            var search = dto.DataPoints.First(x =>
                x.Name.Equals(CommonKeyIndexes.CaseStyle, System.StringComparison.CurrentCultureIgnoreCase));
            var element = driver.FindElement(By.XPath(search.Xpath));
            search.Result = element.Text;
            linkData.PageHtml = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
        }
    }
}