using legallead.harriscriminal.db;
using legallead.records.search.Dto;
using legallead.records.search.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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