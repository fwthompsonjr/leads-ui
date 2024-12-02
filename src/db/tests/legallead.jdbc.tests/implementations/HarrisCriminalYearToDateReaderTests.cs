using legallead.jdbc.implementations;

namespace legallead.jdbc.tests.implementations
{
    public class HarrisCriminalYearToDateReaderTests
    {
        [Theory]
        [InlineData("C:\\_d\\lead-old\\_notes\\Weekly_Historical_Criminal_20241123.zip")]
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
        [InlineData("C:\\_d\\lead-old\\_notes\\Weekly_Historical_Criminal_20241123.zip")]
        public void ServiceCanTranslateZipContent(string sourceFile)
        {
            var error = Record.Exception(() =>
            {
                using var service = new HarrisCriminalYearToDateReader(sourceFile);
                service.Translate();
            });
            Assert.Null(error);
        }
    }
}
