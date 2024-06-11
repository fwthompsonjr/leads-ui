using Bogus;
using legallead.jdbc.entities;
using legallead.jdbc.models;
using Newtonsoft.Json;

namespace legallead.jdbc.tests.entities
{
    public class PricingCodeBoTests
    {
        private static readonly Faker<PricingCodeBo> faker =
            new Faker<PricingCodeBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.PermissionGroupId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyName, y => y.Hacker.Phrase())
            .RuleFor(x => x.ProductCode, y => y.Hacker.Phrase())
            .RuleFor(x => x.PriceCodeAnnual, y => y.Hacker.Phrase())
            .RuleFor(x => x.PriceCodeMonthly, y => y.Hacker.Phrase())
            .RuleFor(x => x.KeyJs, y => ProductGenerator.GetJson())
            .RuleFor(x => x.IsActive, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void PricingCodeBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new PricingCodeBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PricingCodeBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PricingCodeBoCanSetPermissionGroupId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.PermissionGroupId = src.PermissionGroupId;
            Assert.Equal(src.PermissionGroupId, dest.PermissionGroupId);
        }

        [Fact]
        public void PricingCodeBoCanSetKeyName()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.KeyName = src.KeyName;
            Assert.Equal(src.KeyName, dest.KeyName);
        }

        [Fact]
        public void PricingCodeBoCanSetProductCode()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.ProductCode = src.ProductCode;
            Assert.Equal(src.ProductCode, dest.ProductCode);
        }

        [Fact]
        public void PricingCodeBoCanSetPriceCodeAnnual()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.PriceCodeAnnual = src.PriceCodeAnnual;
            Assert.Equal(src.PriceCodeAnnual, dest.PriceCodeAnnual);
        }

        [Fact]
        public void PricingCodeBoCanSetPriceCodeMonthly()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.PriceCodeMonthly = src.PriceCodeMonthly;
            Assert.Equal(src.PriceCodeMonthly, dest.PriceCodeMonthly);
        }

        [Fact]
        public void PricingCodeBoCanSetIsActive()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.IsActive = src.IsActive;
            Assert.Equal(src.IsActive, dest.IsActive);
        }

        [Fact]
        public void PricingCodeBoCanSetKeyJs()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.KeyJs = src.KeyJs;
            Assert.Equal(src.KeyJs, dest.KeyJs);
        }

        [Fact]
        public void PricingCodeBoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Fact]
        public void PricingCodeBoCanDeSerialize()
        {
            var data = faker.Generate();
            var exception = Record.Exception(() =>
            {
                var test = data.GetModel();
                Assert.NotNull(test);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void PricingCodeBoDeSerializeResilience(int conditionId)
        {
            var data = faker.Generate();
            var js = conditionId switch
            {
                0 => null,
                1 => "",
                2 => "     ",
                3 => "Mary had little lamb",
                _ => data.KeyJs
            };

            data.KeyJs = js;
            var exception = Record.Exception(() =>
            {
                _ = data.GetModel();
            });
            Assert.Null(exception);
        }
        private static class ProductGenerator
        {

            private static readonly Faker<BillingProductModel> productfaker =
                new Faker<BillingProductModel>()
                .RuleFor(x => x.Code, y => y.Random.Guid().ToString("D"))
                .RuleFor(x => x.Name, y => y.Hacker.Phrase());

            private static readonly Faker<BillingPriceCodeModel> codefaker =
                new Faker<BillingPriceCodeModel>()
                .RuleFor(x => x.Monthly, y => y.Random.Guid().ToString("D"))
                .RuleFor(x => x.Annual, y => y.Random.Guid().ToString("D"));

            private static readonly Faker<BillingPriceAmountModel> amountfaker =
                new Faker<BillingPriceAmountModel>()
                .RuleFor(x => x.Monthly, y => y.Random.Int(1, 100))
                .RuleFor(x => x.Annual, y => y.Random.Int(1000, 2000));

            private static ProductPricingModel GetModel()
            {
                return new()
                {
                    Product = productfaker.Generate(),
                    PriceAmount = amountfaker.Generate(),
                    PriceCode = codefaker.Generate()
                };
            }

            public static string GetJson()
            {
                var model = GetModel();
                return JsonConvert.SerializeObject(model);
            }
        }
    }
}