using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;
using System.Diagnostics;

namespace legallead.jdbc.tests.implementations
{
    public class HarrisCriminalFileReaderTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("not-an-existing-file-path")]
        public void ServiceReadRequiresFileName(string sourceFile)
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using var service = GetReader(sourceFile);
                service.Read();
            });
        }

        [Theory]
        [InlineData(historyData)]
        public void ServiceCanReadZipContent(string sourceFile)
        {
            if (!Debugger.IsAttached) return;
            var error = Record.Exception(() =>
            {
                using var service = GetReader(sourceFile);
                service.Read();
            });
            Assert.Null(error);
        }

        private static HarrisCriminalFileReader GetReader(string sourceFile)
        {
            var response = new KeyValuePair<bool, string>(true, "test");
            var completion = Task.FromResult(response);
            var mock = new Mock<IHarrisLoadRepository>();
            mock.Setup(x => x.Append(It.IsAny<string>())).Returns(completion);
            return new HarrisCriminalFileReader(sourceFile, mock.Object);
        }
        private const string historyData = "C:\\_d\\lead-old\\_notes\\Weekly_Historical_Criminal.txt";
    }
}