using Bogus;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;
using System.Diagnostics;
using System.Text;

namespace legallead.jdbc.tests.implementations
{
    public class HarrisCriminalTextReaderTests
    {
        [Theory]
        [InlineData("")]
        public void ServiceReadRequiresFileName(string sourceFile)
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using var service = GetReader(sourceFile);
                service.Read();
            });
        }

        [Fact]
        public void ServiceCanReadEncodedString()
        {
            var error = Record.Exception(() =>
            {
                var content = GetEncodedText();
                using var service = GetReader(content);
                service.Read();
            });
            Assert.Null(error);
        }

        [Fact]
        public void ServiceCanNotReadCommonString()
        {
            var error = Record.Exception(() =>
            {
                using var service = GetReader("common text");
                service.Read();
            });
            Assert.Null(error);
        }
        private static HarrisCriminalTextReader GetReader(string sourceFile)
        {
            var response = new KeyValuePair<bool, string>(true, "test");
            var completion = Task.FromResult(response);
            var mock = new Mock<IHarrisLoadRepository>();
            mock.Setup(x => x.Append(It.IsAny<string>())).Returns(completion);
            return new HarrisCriminalTextReader(sourceFile, mock.Object);
        }

        private static string GetEncodedText()
        {
            var phrase = new Faker().Lorem.Paragraphs(3);
            var bytes = Encoding.UTF8.GetBytes(phrase);
            var conversion = Convert.ToBase64String(bytes);
            return conversion;
        }
    }
}