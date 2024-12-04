using Bogus;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;
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

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ServiceCanReadEncodedString(int dataType)
        {
            var error = Record.Exception(() =>
            {
                var content = dataType == 0 ? GetEncodedText() : GetSampleData();
                using var service = GetReader(content);
                service.Read();
                if (dataType == 1) service.Transfer();
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

        private static string GetSampleData()
        {
            var sep = "\t";
            var fields = HarrisCriminalFieldName.Fields.Select(x => x.Key).ToList();
            var contents = new List<string>()
            {
                string.Join(sep, fields),
            };
            for (int i = 0; i < 1005; i++)
            {
                var line = fields.Select(x => Guid.NewGuid().ToString("N"));
                contents.Add(string.Join(sep, line));
            }
            var phrase = string.Join(Environment.NewLine, contents);
            var bytes = Encoding.UTF8.GetBytes(phrase);
            var conversion = Convert.ToBase64String(bytes);
            return conversion;
        }

    }
}