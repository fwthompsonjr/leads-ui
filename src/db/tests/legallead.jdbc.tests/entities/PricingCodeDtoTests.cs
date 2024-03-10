using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class PricingCodeDtoTests
    {

        private static readonly Faker<PricingCodeDto> faker =
            new Faker<PricingCodeDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.PermissionGroupId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyName, y => y.Hacker.Phrase())
            .RuleFor(x => x.ProductCode, y => y.Hacker.Phrase())
            .RuleFor(x => x.PriceCodeAnnual, y => y.Hacker.Phrase())
            .RuleFor(x => x.PriceCodeMonthly, y => y.Hacker.Phrase())
            .RuleFor(x => x.KeyJs, y => y.Hacker.Phrase())
            .RuleFor(x => x.IsActive, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void PricingCodeDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new PricingCodeDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PricingCodeDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PricingCodeDtoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void PricingCodeDtoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("PermissionGroupId")]
        [InlineData("KeyName")]
        [InlineData("ProductCode")]
        [InlineData("PriceCodeAnnual")]
        [InlineData("PriceCodeMonthly")]
        [InlineData("IsActive")]
        [InlineData("KeyJs")]
        [InlineData("CreateDate")]
        public void PricingCodeDtoHasExpectedFieldDefined(string name)
        {
            var sut = new PricingCodeDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("PermissionGroupId")]
        [InlineData("KeyName")]
        [InlineData("ProductCode")]
        [InlineData("PriceCodeAnnual")]
        [InlineData("PriceCodeMonthly")]
        [InlineData("IsActive")]
        [InlineData("KeyJs")]
        [InlineData("CreateDate")]
        public void PricingCodeDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new PricingCodeDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}