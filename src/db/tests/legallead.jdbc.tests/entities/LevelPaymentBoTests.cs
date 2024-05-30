using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class LevelPaymentBoTests
    {


        private static readonly Faker<LevelPaymentBo> faker =
            new Faker<LevelPaymentBo>()
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
        public void LevelPaymentBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LevelPaymentBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LevelPaymentBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LevelPaymentBoCanSetUserId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.UserId = src.UserId;
            Assert.Equal(src.UserId, dest.UserId);
        }

        [Fact]
        public void LevelPaymentBoCanSetExternalId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.ExternalId = src.ExternalId;
            Assert.Equal(src.ExternalId, dest.ExternalId);
        }

        [Fact]
        public void LevelPaymentBoCanSetLevelName()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.LevelName = src.LevelName;
            Assert.Equal(src.LevelName, dest.LevelName);
        }

        [Fact]
        public void LevelPaymentBoCanSetPriceType()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.PriceType = src.PriceType;
            Assert.Equal(src.PriceType, dest.PriceType);
        }

        [Fact]
        public void LevelPaymentBoCanSetPrice()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Price = src.Price;
            Assert.Equal(src.Price, dest.Price);
        }

        [Fact]
        public void LevelPaymentBoCanSetTaxAmount()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.TaxAmount = src.TaxAmount;
            Assert.Equal(src.TaxAmount, dest.TaxAmount);
        }

        [Fact]
        public void LevelPaymentBoCanSetServiceFee()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.ServiceFee = src.ServiceFee;
            Assert.Equal(src.ServiceFee, dest.ServiceFee);
        }

        [Fact]
        public void LevelPaymentBoCanSetSubscriptionAmount()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.SubscriptionAmount = src.SubscriptionAmount;
            Assert.Equal(src.SubscriptionAmount, dest.SubscriptionAmount);
        }

        [Fact]
        public void LevelPaymentBoCanSetCompletionDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CompletionDate = src.CompletionDate;
            Assert.Equal(src.CompletionDate, dest.CompletionDate);
        }

        [Fact]
        public void LevelPaymentBoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }
    }
}