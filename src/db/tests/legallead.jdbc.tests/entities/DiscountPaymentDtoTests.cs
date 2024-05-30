using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class DiscountPaymentDtoTests
    {

        private static readonly Faker<DiscountPaymentDto> faker =
            new Faker<DiscountPaymentDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExternalId, y => y.Hacker.Phrase())
            .RuleFor(x => x.LevelName, y => y.Hacker.Phrase())
            .RuleFor(x => x.PriceType, y => y.Hacker.Phrase())
            .RuleFor(x => x.Price, y => y.Random.Decimal(1, 50))
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void DiscountPaymentDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new DiscountPaymentDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void DiscountPaymentDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void DiscountPaymentDtoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void DiscountPaymentDtoCanSetCreateDate()
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
        [InlineData("CompletionDate")]
        [InlineData("CreateDate")]
        public void DiscountPaymentDtoHasExpectedFieldDefined(string name)
        {
            var sut = new DiscountPaymentDto();
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
        [InlineData("CompletionDate")]
        [InlineData("CreateDate")]
        public void DiscountPaymentDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new DiscountPaymentDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}