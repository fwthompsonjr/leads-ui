using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class LevelPaymentDtoTests
    {

        private static readonly Faker<LevelPaymentDto> faker =
            new Faker<LevelPaymentDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExternalId, y => y.Hacker.Phrase())
            .RuleFor(x => x.LevelName, y => y.Hacker.Phrase())
            .RuleFor(x => x.PriceType, y => y.Hacker.Phrase())
            .RuleFor(x => x.Price, y => y.Random.Decimal(1, 50))
            .RuleFor(x => x.TaxAmount, y => y.Random.Decimal(1, 10))
            .RuleFor(x => x.ServiceFee, y => y.Random.Decimal(1, 10))
            .RuleFor(x => x.SubscriptionAmount, y => y.Random.Decimal(50, 100))
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void LevelPaymentDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LevelPaymentDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LevelPaymentDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LevelPaymentDtoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void LevelPaymentDtoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("ExternalId")]
        [InlineData("LevelName")]
        [InlineData("PriceType")]
        [InlineData("Price")]
        [InlineData("TaxAmount")]
        [InlineData("ServiceFee")]
        [InlineData("SubscriptionAmount")]
        [InlineData("CompletionDate")]
        [InlineData("CreateDate")]
        public void LevelPaymentDtoHasExpectedFieldDefined(string name)
        {
            var sut = new LevelPaymentDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("ExternalId")]
        [InlineData("LevelName")]
        [InlineData("PriceType")]
        [InlineData("Price")]
        [InlineData("TaxAmount")]
        [InlineData("ServiceFee")]
        [InlineData("SubscriptionAmount")]
        [InlineData("CompletionDate")]
        [InlineData("CreateDate")]
        public void LevelPaymentDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new LevelPaymentDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}