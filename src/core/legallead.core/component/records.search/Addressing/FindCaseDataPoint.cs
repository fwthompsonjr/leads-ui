using legallead.records.search.Dto;
using legallead.records.search.Models;
using OpenQA.Selenium;

namespace legallead.records.search.Addressing
{
    public class FindCaseDataPoint : FindDefendantBase
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
            DataPointLocatorDto dto = DataPointLocatorDto.GetDto("tarrantCountyDataPoint");
            DataPoint search = dto.DataPoints.First(x =>
                x.Name.Equals(CommonKeyIndexes.CaseStyle, System.StringComparison.CurrentCultureIgnoreCase));
            IWebElement element = driver.FindElement(By.XPath(search.Xpath));
            search.Result = element.Text;
            linkData.PageHtml = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
        }
    }
}