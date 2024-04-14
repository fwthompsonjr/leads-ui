using legallead.permissions.api.Models;

namespace permissions.api.tests.Models
{
    public class DownloadRequestTests
    {
        private static readonly Faker<DownloadRequest> faker =
            new Faker<DownloadRequest>()
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString("D"));


        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new DownloadRequest();
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
        public void ModelCanGetField(int fieldId)
        {
            var test = faker.Generate();
            var control = new DownloadRequest();
            if (fieldId == 0) Assert.NotEqual(control.ExternalId, test.ExternalId);
        }
    }
}