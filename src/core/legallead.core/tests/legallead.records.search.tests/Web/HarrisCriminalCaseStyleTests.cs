using legallead.harriscriminal.db;
using legallead.records.search.Dto;
using legallead.records.search.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Shouldly;

namespace legallead.records.search.Tests
{
    [TestClass]
    public class HarrisCriminalCaseStyleTests : TestingBase
    {
        private readonly int MxCaseNumbers = 10;
        private List<HarrisCaseSearchDto?>? CaseNumbers;
        private string? MaximumFileDate;
        private string? MinimumFileDate;

        [TestInitialize]
        public void Setup()
        {
            if (CaseNumbers == null)
            {
                Startup.Downloads.ReadFiles();
                var datalist = Startup.Downloads.DataList.FirstOrDefault();
                if (datalist == null) return;
                var dtos = datalist.Data.Select(x =>
                    new HarrisCaseSearchDto
                    {
                        CaseNumber = x.CaseNumber,
                        Court = x.Court,
                        DateFiled = x.FilingDate.ToExactDateString("yyyyMMdd", string.Empty)
                    });
                var list = dtos.GroupBy(x => x.UniqueIndex()).Select(x => x.FirstOrDefault());
                CaseNumbers = list.Take(MxCaseNumbers).ToList();
                MaximumFileDate = list.Max(x => x!.DateFiled ?? string.Empty);
                MinimumFileDate = list.Min(x => x!.DateFiled ?? string.Empty);
            }
        }

        [TestMethod]
        public void DownloadHasACorrectTarget()
        {
            var folder = HarrisCriminalCaseStyle.DownloadFolder;
            folder.ShouldNotBeNullOrEmpty();
            Directory.Exists(folder).ShouldBeTrue();
        }

        [TestMethod]
        [TestCategory("Integration Only")]
        public void CaseStyleCanGetSingleRecord()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Assert.Inconclusive("This method to be executed in debug mode only.");
            }
            if (CaseNumbers == null) return;
            var caseNumber = CaseNumbers[^1];
            caseNumber.ShouldNotBeNull();
            var dto = new HarrisCaseSearchDto { CaseNumber = caseNumber.CaseNumber };
            var obj = new HarrisCriminalCaseStyle();
            IWebDriver driver = GetDriver();
            try
            {
                var result = obj.GetData(driver, dto);
                result.ShouldNotBeNull();
                result.Count.ShouldBeGreaterThan(0);
            }
            finally
            {
                driver?.Close();
                driver?.Quit();
                KillProcess("chromedriver");
            }
        }

        [TestMethod]
        [TestCategory("Integration Only")]
        public async Task CaseStyleCanGetBulkAsync()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Assert.Inconclusive("This method to be executed in debug mode only.");
            }
            if (MaximumFileDate == null) return;
            const string fmt = "yyyyMMdd";
            DateTime dateBase = DateTime.MaxValue;
            var dtmin = MaximumFileDate.ToExactDate(fmt, dateBase).AddDays(-10);
            var dtmax = MaximumFileDate.ToExactDate(fmt, dateBase);
            dtmin.ShouldNotBe(dateBase);
            var dateRange = Convert.ToInt32(dtmax.Subtract(dtmin).TotalDays) + 1;

            var obj = new HarrisCriminalCaseStyle();
            IWebDriver driver = GetDriver();
            var result = new List<HarrisCriminalStyleDto>();
            try
            {
                var records = await Task.Run(() => { return obj.GetCases(driver, dtmax, dateRange); });
                result.Append(records);
                result.ShouldNotBeNull();
                result.Count.ShouldBeGreaterThan(0);
            }
            finally
            {
                driver?.Close();
                driver?.Quit();
                KillProcess("chromedriver");
            }
        }

        [TestMethod]
        [TestCategory("Integration Only")]
        public async Task CaseStyleCanStartAsync()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Assert.Inconclusive("This method to be executed in debug mode only.");
            }
            try
            {
                var starting = new List<CriminalStartType> { CriminalStartType.CaseTypes };
                await HarrisCriminalStarting.StartAsync(starting);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + Environment.NewLine + exception.StackTrace);
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Integration Only")]
        public async Task CaseStyleCanGetRealTimeAsync()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Assert.Inconclusive("This method to be executed in debug mode only.");
            }
            DateTime MxDate = DateTime.Now.AddDays(-1).Date;
            DateTime MnDate = MxDate.AddDays(-3);

            var obj = new HarrisCriminalCaseStyle();
            IWebDriver driver = GetDriver();
            var result = new List<HarrisCriminalStyleDto>();
            try
            {
                var records = await Task.Run(() => { return obj.GetCases(driver, MnDate, MxDate); });
                result.Append(records);
                result.ShouldNotBeNull();
                result.Count.ShouldBeGreaterThan(0);
            }
            finally
            {
                driver?.Close();
                driver?.Quit();
                KillProcess("chromedriver");
            }
        }

        [TestMethod]
        public void CanParseMinMax()
        {
            if (MinimumFileDate == null || MaximumFileDate == null) return;
            const string fmt = "yyyyMMdd";
            DateTime dateBase = DateTime.MaxValue;
            var dtmin = MinimumFileDate.ToExactDate(fmt, dateBase);
            var dtmax = MaximumFileDate.ToExactDate(fmt, dateBase);
            dtmin.ShouldNotBe(dateBase);
            dtmax.ShouldNotBe(dateBase);
            dtmin.ShouldBeLessThanOrEqualTo(dtmax);
        }
    }
}