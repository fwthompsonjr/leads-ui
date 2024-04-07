using Bogus;
using legallead.installer.Classes;

namespace legallead.installer.tests
{
    public class ZipProgressTests
    {
        private static readonly Faker<ZipProgressModel> faker = new Faker<ZipProgressModel>()
            .RuleFor(x => x.Total, y => y.Random.Int(101, 150))
            .RuleFor(x => x.Processed, y => y.Random.Int(1, 100))
            .RuleFor(x => x.CurrentItem, y => y.System.FileName());

        [Fact]
        public void ModelCanBeCreated()
        {
            var item = faker.Generate();
            var converted = item.GetZipProgress();
            Assert.False(string.IsNullOrEmpty(converted.CurrentItem));
            Assert.True(converted.Total > converted.Processed);
        }

        private sealed class ZipProgressModel
        {

            public int Total { get; set; }
            public int Processed { get; set; }
            public string CurrentItem { get; set; } = string.Empty;
            public ZipProgress GetZipProgress()
            {
                return new ZipProgress(Total, Processed, CurrentItem);
            }
        }
    }
}
