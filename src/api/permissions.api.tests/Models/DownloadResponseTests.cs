using legallead.permissions.api.Models;
using System.Text;

namespace permissions.api.tests.Models
{
    public class DownloadResponseTests
    {
        private static readonly Faker<DownloadResponse> faker =
            new Faker<DownloadResponse>()
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Description, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Error, y => y.Hacker.Phrase())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent().ToString("s"))
            .FinishWith((a, b) => {
                b.Content = Encoding.UTF8.GetBytes(b.Error ?? string.Empty);
            });


        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new DownloadResponse();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanGenerate()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void ModelCanGetField(int fieldId)
        {
            var test = faker.Generate();
            var control = new DownloadResponse();
            if (fieldId == 0) Assert.NotEqual(control.ExternalId, test.ExternalId);
            if (fieldId == 1) Assert.NotEqual(control.Description, test.Description);
            if (fieldId == 2) Assert.NotEqual(control.Content, test.Content);
            if (fieldId == 3) Assert.NotEqual(control.Error, test.Error);
            if (fieldId == 4) Assert.NotEqual(control.CreateDate, test.CreateDate);
        }
    }
}