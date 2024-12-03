using legallead.jdbc.helpers;
using legallead.jdbc.implementations;

namespace legallead.jdbc.tests.implementations
{
    public class HarrisCriminalYearToDateReaderTests
    {
        [Theory]
        [InlineData(historyData)]
        public void ServiceCanReadZipContent(string sourceFile)
        {
            var error = Record.Exception(() =>
            {
                using var service = new HarrisCriminalYearToDateReader(sourceFile);
                service.Read();
            });
            Assert.Null(error);
        }
        [Theory]
        [InlineData(historyData)]
        public void ServiceCanTranslateZipContent(string sourceFile)
        {
            var error = Record.Exception(() =>
            {
                using var service = new HarrisCriminalYearToDateReader(sourceFile);
                service.Translate();
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(historyData)]
        public void ServiceCanTransferZipContent(string sourceFile)
        {
            var error = Record.Exception(() =>
            {
                using var service = new HarrisCriminalYearToDateReader(sourceFile);
                service.Transfer();
            });
            Assert.Null(error);
        }

        [Fact]
        public void LookupServiceCanMapData()
        {
            var error = Record.Exception(() =>
            {
                _ = HarrisLookupService.Data;
            });
            Assert.Null(error);
        }

        private const string historyData = "C:\\_d\\lead-old\\_notes\\Weekly_Historical_Criminal.zip";
    }
}
