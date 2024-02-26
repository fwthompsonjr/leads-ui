using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class PurchasedSearchDtoTests
    {

        private static readonly Faker<PurchasedSearchDto> faker =
            new Faker<PurchasedSearchDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.PurchaseDate, y => y.Date.Recent())
            .RuleFor(x => x.ReferenceId, y => y.Hacker.Phrase())
            .RuleFor(x => x.ExternalId, y => y.Hacker.Phrase())
            .RuleFor(x => x.ItemType, y => y.Hacker.Phrase())
            .RuleFor(x => x.StatusText, y => y.Hacker.Phrase())
            .RuleFor(x => x.ItemCount, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.Price, y => y.Random.Int(1, 1000));


        [Fact]
        public void PurchasedSearchDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new PurchasedSearchDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PurchasedSearchDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }


        [Fact]
        public void SearchCanSetPurchaseDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.PurchaseDate = src.PurchaseDate;
            Assert.Equal(src.PurchaseDate, dest.PurchaseDate);
        }

        [Fact]
        public void SearchCanSetReferenceId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.ReferenceId = src.ReferenceId;
            Assert.Equal(src.ReferenceId, dest.ReferenceId);
        }

        [Fact]
        public void SearchCanSetExternalId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.ExternalId = src.ExternalId;
            Assert.Equal(src.ExternalId, dest.ExternalId);
        }

        [Fact]
        public void SearchCanSetItemType()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.ItemType = src.ItemType;
            Assert.Equal(src.ItemType, dest.ItemType);
        }

        [Fact]
        public void SearchCanSetItemCount()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.ItemCount = src.ItemCount;
            Assert.Equal(src.ItemCount, dest.ItemCount);
        }

        [Fact]
        public void SearchCanSetPrice()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Price = src.Price;
            Assert.Equal(src.Price, dest.Price);
        }

        [Fact]
        public void SearchCanSetStatusText()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.StatusText = src.StatusText;
            Assert.Equal(src.StatusText, dest.StatusText);
        }



        [Fact]
        public void PurchasedSearchDtoIsBaseDto()
        {
            var sut = new PurchasedSearchDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("PurchaseDate")]
        [InlineData("ReferenceId")]
        [InlineData("ExternalId")]
        [InlineData("ItemType")]
        [InlineData("StatusText")]
        [InlineData("ItemCount")]
        [InlineData("Price")]
        public void PurchasedSearchDtoHasExpectedFieldDefined(string name)
        {
            var sut = new PurchasedSearchDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("PurchaseDate")]
        [InlineData("ReferenceId")]
        [InlineData("ExternalId")]
        [InlineData("ItemType")]
        [InlineData("StatusText")]
        [InlineData("ItemCount")]
        [InlineData("Price")]
        public void PurchasedSearchDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new PurchasedSearchDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}