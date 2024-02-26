using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class PurchasedSearchBoTests
    {

        private static readonly Faker<PurchasedSearchBo> faker =
            new Faker<PurchasedSearchBo>()
            .RuleFor(x => x.PurchaseDate, y => y.Date.Recent())
            .RuleFor(x => x.ReferenceId, y => y.Hacker.Phrase())
            .RuleFor(x => x.ExternalId, y => y.Hacker.Phrase())
            .RuleFor(x => x.ItemType, y => y.Hacker.Phrase())
            .RuleFor(x => x.StatusText, y => y.Hacker.Phrase())
            .RuleFor(x => x.ItemCount, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.Price, y => y.Random.Int(1, 1000));


        [Fact]
        public void PurchasedSearchBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new PurchasedSearchBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PurchasedSearchBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PurchasedSearchBoCanSetPurchaseDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.PurchaseDate = src.PurchaseDate;
            Assert.Equal(src.PurchaseDate, dest.PurchaseDate);
        }

        [Fact]
        public void PurchasedSearchBoCanSetReferenceId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.ReferenceId = src.ReferenceId;
            Assert.Equal(src.ReferenceId, dest.ReferenceId);
        }

        [Fact]
        public void PurchasedSearchBoCanSetExternalId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.ExternalId = src.ExternalId;
            Assert.Equal(src.ExternalId, dest.ExternalId);
        }

        [Fact]
        public void PurchasedSearchBoCanSetItemType()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.ItemType = src.ItemType;
            Assert.Equal(src.ItemType, dest.ItemType);
        }

        [Fact]
        public void PurchasedSearchBoCanSetItemCount()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.ItemCount = src.ItemCount;
            Assert.Equal(src.ItemCount, dest.ItemCount);
        }

        [Fact]
        public void PurchasedSearchBoCanSetPrice()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Price = src.Price;
            Assert.Equal(src.Price, dest.Price);
        }

        [Fact]
        public void PurchasedSearchBoCanSetStatusText()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.StatusText = src.StatusText;
            Assert.Equal(src.StatusText, dest.StatusText);
        }
    }
}