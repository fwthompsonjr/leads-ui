using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class SearchPreviewDtoTests
    {

        private static readonly Faker<SearchPreviewDto> faker =
            new Faker<SearchPreviewDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Zip, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Address1, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Address2, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Address3, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CaseNumber, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.DateFiled, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Court, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CaseType, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CaseStyle, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.FirstName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LastName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Plantiff, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Status, y => y.Random.Guid().ToString("D"));

        [Fact]
        public void SearchPreviewDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchPreviewDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchPreviewDtoCanBeGenerated()
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
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        public void SearchPreviewDtoCanWriteAndRead(int fieldId)
        {
            var exception = Record.Exception(() =>
            {
                var a = faker.Generate();
                var b = faker.Generate();
                a[fieldId] = b[fieldId];
                Assert.Equal(a[fieldId], b[fieldId]);
            });
            Assert.Null(exception);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        public void SearchPreviewDtoCanWriteAndReadByFieldName(int fieldId)
        {
            var exception = Record.Exception(() =>
            {
                var a = faker.Generate();
                var b = faker.Generate();
                var name = a.FieldList[fieldId];
                a[name] = b[name];
                Assert.Equal(a[name], b[name]);
            });
            Assert.Null(exception);
        }
    }
}