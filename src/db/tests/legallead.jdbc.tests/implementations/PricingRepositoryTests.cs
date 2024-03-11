using Bogus;
using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using Moq;
using Newtonsoft.Json;
using System.Data;

namespace legallead.jdbc.tests.implementations
{
    public class PricingRepositoryTests
    {

        private static readonly Faker<PricingCodeDto> faker =
            new Faker<PricingCodeDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.PermissionGroupId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyName, y => y.Hacker.Phrase())
            .RuleFor(x => x.ProductCode, y => y.Hacker.Phrase())
            .RuleFor(x => x.PriceCodeAnnual, y => y.Hacker.Phrase())
            .RuleFor(x => x.PriceCodeMonthly, y => y.Hacker.Phrase())
            .RuleFor(x => x.KeyJs, y => ProductGenerator.GetJson())
            .RuleFor(x => x.IsActive, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

#pragma warning disable CS8604 // Possible null reference argument.

        [Fact]
        public void RepoCanBeConstructed()
        {
            var provider = new PricingRepoContainer();
            var repo = provider.PricingRepo;
            Assert.NotNull(repo);
        }

        [Fact]
        public async Task CreatePricingTemplateHappyPath()
        {
            var completion = faker.Generate();
            var provider = new PricingRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.PricingRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<PricingCodeDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.CreatePricingTemplate(completion.Id, new());
            Assert.NotNull(response);
        }

        [Fact]
        public async Task CreatePricingTemplateNoResponse()
        {
            PricingCodeDto? completion = default;
            var provider = new PricingRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.PricingRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<PricingCodeDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.CreatePricingTemplate("", new());
            Assert.Null(response);
        }

        [Fact]
        public async Task CreatePricingTemplateExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new PricingRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.PricingRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<PricingCodeDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.CreatePricingTemplate("", new());
            Assert.Null(response);
        }


        [Fact]
        public async Task SetActivePricingTemplateHappyPath()
        {
            var completion = faker.Generate();
            var provider = new PricingRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.PricingRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<PricingCodeDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.SetActivePricingTemplate(completion.Id, ProductGenerator.GetModel());
            Assert.NotNull(response);
        }

        [Fact]
        public async Task SetActivePricingTemplateJsonHappyPath()
        {
            var completion = faker.Generate();
            var modeljs = JsonConvert.SerializeObject(ProductGenerator.GetModel());
            var provider = new PricingRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.PricingRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<PricingCodeDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.SetActivePricingTemplate(completion.Id, modeljs);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task SetActivePricingTemplateJsonNoDataPath()
        {
            var provider = new PricingRepoContainer();
            var service = provider.PricingRepo;
            var response = await service.SetActivePricingTemplate("", "abcdefg");
            Assert.Null(response);
        }

        [Fact]
        public async Task SetActivePricingTemplateNoResponse()
        {
            PricingCodeDto? completion = default;
            var provider = new PricingRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.PricingRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<PricingCodeDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.SetActivePricingTemplate("", ProductGenerator.GetModel());
            Assert.Null(response);
        }

        [Fact]
        public async Task SetActivePricingTemplateExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new PricingRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.PricingRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<PricingCodeDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.SetActivePricingTemplate("", ProductGenerator.GetModel());
            Assert.Null(response);
        }

        [Theory]
        [InlineData("model.Product.Code")]
        [InlineData("model.PriceCode.Annual")]
        [InlineData("model.PriceCode.Monthly")]
        public async Task SetActivePricingTemplateArgumentValidations(string fieldChange)
        {
            var completion = faker.Generate();
            var model = ProductGenerator.GetModel();
            switch (fieldChange)
            {
                case "model.Product.Code":
                    model.Product.Code = string.Empty;
                    break;
                case "model.PriceCode.Annual":
                    model.PriceCode.Annual = string.Empty;
                    break;
                case "model.PriceCode.Monthly":
                    model.PriceCode.Monthly = string.Empty;
                    break;
                default:
                    break;
            }
            var provider = new PricingRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.PricingRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<PricingCodeDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var exception = await Record.ExceptionAsync(async () => {
                await service.SetActivePricingTemplate("", model);
            });
            Assert.NotNull(exception);
            Assert.IsAssignableFrom<ArgumentOutOfRangeException>(exception);
        }

        [Fact]
        public async Task GetPricingTemplateHistoryHappyPath()
        {
            var completion = faker.Generate(5);
            var provider = new PricingRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.PricingRepo;
            mock.Setup(m => m.QueryAsync<PricingCodeDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.GetPricingTemplateHistory();
            Assert.NotEmpty(response);

        }

        [Fact]
        public async Task GetPricingTemplateHistoryNoResponse()
        {
            List<PricingCodeDto>? completion = default;
            var provider = new PricingRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.PricingRepo;
            mock.Setup(m => m.QueryAsync<PricingCodeDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.GetPricingTemplateHistory();
            Assert.Empty(response);
        }

        [Fact]
        public async Task GetPricingTemplateHistoryExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new PricingRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.PricingRepo;
            mock.Setup(m => m.QueryAsync<PricingCodeDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.GetPricingTemplateHistory();
            Assert.Empty(response);
        }

        [Fact]
        public async Task GetPricingTemplatesHappyPath()
        {
            var completion = faker.Generate(5);
            var provider = new PricingRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.PricingRepo;
            mock.Setup(m => m.QueryAsync<PricingCodeDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.GetPricingTemplates();
            Assert.NotNull(response);
        }

        [Fact]
        public async Task GetPricingTemplatesNoResponse()
        {
            List<PricingCodeDto>? completion = default;
            var provider = new PricingRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.PricingRepo;
            mock.Setup(m => m.QueryAsync<PricingCodeDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.GetPricingTemplates();
            Assert.Empty(response);
        }

        [Fact]
        public async Task GetPricingTemplatesExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new PricingRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.PricingRepo;
            mock.Setup(m => m.QueryAsync<PricingCodeDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.GetPricingTemplates();
            Assert.Empty(response);
        }
#pragma warning restore CS8604 // Possible null reference argument.

        private sealed class PricingRepoContainer
        {
            private readonly IPricingRepository repo;
            private readonly Mock<IDapperCommand> command;
            public PricingRepoContainer()
            {

                command = new Mock<IDapperCommand>();
                var dataContext = new DataContext(command.Object);
                repo = new PricingRepository(dataContext);
            }

            public IPricingRepository PricingRepo => repo;
            public Mock<IDapperCommand> CommandMock => command;
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

            public static ProductPricingModel GetModel()
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