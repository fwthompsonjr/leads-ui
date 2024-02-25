using Bogus;
using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;
using System.Data;
using System.Text;

namespace legallead.jdbc.tests.implementations
{
    public class UserSearchRepositoryTests
    {
        private static readonly Faker<SearchFinalDto> finalfaker =
            new Faker<SearchFinalDto>()
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
            .RuleFor(x => x.Status, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.County, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CourtAddress, y => y.Random.Guid().ToString("D"));

        private static readonly Faker<SearchPreviewDto> previewfaker =
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

        private static readonly Faker<SearchInvoiceDto> invoicefaker =
            new Faker<SearchInvoiceDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LineId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ItemType, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ItemCount, y => y.Random.Int(1, 25))
            .RuleFor(x => x.UnitPrice, y => y.Random.Int(1, 25))
            .RuleFor(x => x.Price, y => y.Random.Int(1, 25))
            .RuleFor(x => x.ReferenceId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.PurchaseDate, y => y.Date.Recent())
            .RuleFor(x => x.IsDeleted, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        private static readonly Faker faker = new();
        [Fact]
        public void RepoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                var container = new RepoContainer();
                Assert.NotNull(container.Repo);
            });
            Assert.Null(exception);
        }


        [Fact]
        public async Task RepoGetSearchRestrictionNoResult()
        {
            var result = new SearchRestrictionDto();
            var uid = faker.Random.Guid().ToString();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<SearchRestrictionDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            await service.GetSearchRestriction(uid);
            mock.Verify(m => m.QuerySingleOrDefaultAsync<SearchRestrictionDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }

        [Fact]
        public async Task RepoGetInvoiceDescriptionHappyPath()
        {
            var result = new InvoiceDescriptionDto();
            var uid = faker.Random.Guid().ToString();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<InvoiceDescriptionDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            await service.InvoiceDescription(uid);
            mock.Verify(m => m.QuerySingleOrDefaultAsync<InvoiceDescriptionDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }


        [Fact]
        public async Task RepoGetPaymentSessionHappyPath()
        {
            var result = new PaymentSessionDto();
            var uid = faker.Random.Guid().ToString();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<PaymentSessionDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            await service.GetPaymentSession(uid);
            mock.Verify(m => m.QuerySingleOrDefaultAsync<PaymentSessionDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }

        [Fact]
        public async Task RepoContentExceptionAreCaught()
        {
            var exception = faker.System.Exception;
            var type = faker.PickRandom<SearchTargetTypes>();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).Throws(exception);
            _ = await service.Append(type, "abc123", Encoding.UTF8.GetBytes("message"));
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Fact]
        public async Task RepoCreateInvoiceHappyPath()
        {
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
            var result = await service.CreateInvoice("abc123", "xyz223");
            Assert.True(result);
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Fact]
        public async Task RepoCreateInvoiceExceptionPath()
        {
            var exception = faker.System.Exception;
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).Throws(exception);
            var result = await service.CreateInvoice("abc123", "xyz223");
            Assert.False(result);
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Fact]
        public async Task RepoRequeueSearchesHappyPath()
        {
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
            var result = await service.RequeueSearches();
            Assert.True(result);
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Fact]
        public async Task RepoRequeueSearchesExceptionPath()
        {
            var exception = faker.System.Exception;
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).Throws(exception);
            var result = await service.RequeueSearches();
            Assert.False(result);
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Fact]
        public async Task RepoUpdateSearchRowCountHappyPath()
        {
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
            var result = await service.UpdateSearchRowCount();
            Assert.True(result);
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Fact]
        public async Task RepoUpdateSearchRowCountExceptionPath()
        {
            var exception = faker.System.Exception;
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).Throws(exception);
            var result = await service.UpdateSearchRowCount();
            Assert.False(result);
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Fact]
        public async Task RepoHistoryNoResult()
        {
            SearchQueryDto[] result = Array.Empty<SearchQueryDto>();
            var uid = faker.Random.Guid().ToString();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<SearchQueryDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            await service.History(uid);
            mock.Verify(m => m.QueryAsync<SearchQueryDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }

        [Fact]
        public async Task RepoHistoryMultipleResult()
        {
            SearchQueryDto[] result = new[] {
                new SearchQueryDto (),
                new SearchQueryDto ()
            };
            var uid = faker.Random.Guid().ToString();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<SearchQueryDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            await service.History(uid);
            mock.Verify(m => m.QueryAsync<SearchQueryDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }

        [Fact]
        public async Task RepoPreviewNoResult()
        {
            SearchPreviewDto[] result = Array.Empty<SearchPreviewDto>();
            var uid = faker.Random.Guid().ToString();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<SearchPreviewDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            await service.Preview(uid);
            mock.Verify(m => m.QueryAsync<SearchPreviewDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }

        [Fact]
        public async Task RepoPreviewMultipleResult()
        {
            SearchPreviewDto[] result = previewfaker.Generate(6).ToArray();
            var uid = faker.Random.Guid().ToString();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<SearchPreviewDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            var translated = (await service.Preview(uid)).ToList();
            Assert.Equal(result[1].Court, translated[1].Court);
            Assert.Equal(result[1].SearchId, translated[1].SearchId);
            Assert.Equal(result[1].Name, translated[1].Name);
            Assert.Equal(result[1].Address1, translated[1].Address1);
            Assert.Equal(result[1].DateFiled, translated[1].DateFiled);
            Assert.Equal(result[1].FirstName, translated[1].FirstName);
            mock.Verify(m => m.QueryAsync<SearchPreviewDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }


        [Fact]
        public async Task RepoGetFinalNoResult()
        {
            SearchFinalDto[] result = Array.Empty<SearchFinalDto>();
            var uid = faker.Random.Guid().ToString();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<SearchFinalDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            await service.GetFinal(uid);
            mock.Verify(m => m.QueryAsync<SearchFinalDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }

        [Fact]
        public async Task RepoGetFinalMultipleResult()
        {
            SearchFinalDto[] result = finalfaker.Generate(6).ToArray();
            var uid = faker.Random.Guid().ToString();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<SearchFinalDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            var translated = (await service.GetFinal(uid)).ToList();
            Assert.Equal(result[1].Court, translated[1].Court);
            Assert.Equal(result[1].SearchId, translated[1].SearchId);
            Assert.Equal(result[1].Name, translated[1].Name);
            Assert.Equal(result[1].Address1, translated[1].Address1);
            Assert.Equal(result[1].DateFiled, translated[1].DateFiled);
            Assert.Equal(result[1].FirstName, translated[1].FirstName);
            mock.Verify(m => m.QueryAsync<SearchFinalDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }


        [Theory]
        [InlineData(SearchTargetTypes.Detail)]
        [InlineData(SearchTargetTypes.Request)]
        [InlineData(SearchTargetTypes.Response)]
        [InlineData(SearchTargetTypes.Status)]
        [InlineData(SearchTargetTypes.Staging)]
        public async Task RepoGetTargetsWithMultipleResults(SearchTargetTypes type)
        {
            var result = new[] {
                new SearchTargetDto (),
                new SearchTargetDto ()
            };
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<SearchTargetDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).ReturnsAsync(result);
            _ = await service.GetTargets(type, "abc123", "xyz123");
            mock.Verify(m => m.QueryAsync<SearchTargetDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Fact]
        public async Task RepoGetStagedWithMultipleResults()
        {
            var result = new[] {
                new SearchStagingDto (),
                new SearchStagingDto ()
            };
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<SearchStagingDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).ReturnsAsync(result);
            _ = await service.GetStaged("abc123", "xyz123");
            mock.Verify(m => m.QueryAsync<SearchStagingDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Fact]
        public async Task RepoGetStagedWillHandleExceptions()
        {
            var result = faker.System.Exception;
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<SearchStagingDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).Throws(result);
            _ = await service.GetStaged("abc123", "xyz123");
            mock.Verify(m => m.QueryAsync<SearchStagingDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }


        [Fact]
        public async Task RepoAppendPaymentSessionHappyPath()
        {
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
            var result = await service.AppendPaymentSession(new());
            Assert.True(result);
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Fact]
        public async Task RepoAppendPaymentSessionExceptionPath()
        {
            var exception = faker.System.Exception;
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).Throws(exception);
            var result = await service.AppendPaymentSession(new());
            Assert.False(result);
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }
        [Theory]
        [InlineData(null)]
        [InlineData("abc")]
        [InlineData("123")]
        public async Task RepoInvoicesMultipleResult(string? searchid)
        {
            var result = invoicefaker.Generate(6).ToArray();
            var uid = faker.Random.Guid().ToString();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<SearchInvoiceDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            _ = await service.Invoices(uid, searchid);
            mock.Verify(m => m.QueryAsync<SearchInvoiceDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("123")]
        public async Task RepoGetActiveSearchesHappyPath(string searchid)
        {
            var result = new ActiveSearchOverviewDto();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<ActiveSearchOverviewDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            _ = await service.GetActiveSearches(searchid);
            mock.Verify(m => m.QuerySingleOrDefaultAsync<ActiveSearchOverviewDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }

        [Theory]
        [InlineData("000")]
        [InlineData("abc")]
        [InlineData("123")]
        public async Task RepoGetActiveSearchDetailsHappyPath(string searchid)
        {
            var result = searchid == "000" ?
                Array.Empty<ActiveSearchDetailDto>() :
                new[] {
                    new ActiveSearchDetailDto(),
                    new ActiveSearchDetailDto(),
                    };
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<ActiveSearchDetailDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            _ = await service.GetActiveSearchDetails(searchid);
            mock.Verify(m => m.QueryAsync<ActiveSearchDetailDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RepoIsValidExternalId(bool? searchid)
        {
            IsValidExternalIndexDto? result =
                searchid == null ? null :
                new IsValidExternalIndexDto { IsFound = searchid };
            var uid = faker.Random.Guid().ToString();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<IsValidExternalIndexDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            _ = await service.IsValidExternalId(uid);
            mock.Verify(m => m.QuerySingleOrDefaultAsync<IsValidExternalIndexDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }

        [Fact]
        public async Task RepoGetPurchaseSummary()
        {
            var result = new PurchaseSummaryDto();
            var uid = faker.Random.Guid().ToString();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<PurchaseSummaryDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            _ = await service.GetPurchaseSummary(uid);
            mock.Verify(m => m.QuerySingleOrDefaultAsync<PurchaseSummaryDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }

        [Fact]
        public async Task RepoSetInvoicePurchaseDateHappyPath()
        {
            var uid = faker.Random.Guid().ToString();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
            var response = await service.SetInvoicePurchaseDate(uid);
            Assert.True(response);
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }

        [Fact]
        public async Task RepoSetInvoicePurchaseDateErrorPath()
        {
            var exception = faker.System.Exception();
            var uid = faker.Random.Guid().ToString();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).Throws(exception);
            var response = await service.SetInvoicePurchaseDate(uid);
            Assert.False(response);
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }


        private sealed class RepoContainer
        {
            private readonly UserSearchRepository repo;
            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {
                command = new Mock<IDapperCommand>();
                var dataContext = new MockDataContext(command.Object);
                repo = new UserSearchRepository(dataContext);
            }

            public UserSearchRepository Repo => repo;
            public Mock<IDapperCommand> CommandMock => command;

        }
        private sealed class MockDataContext : DataContext
        {
            public MockDataContext(IDapperCommand command) : base(command)
            {
            }

            public override IDbConnection CreateConnection()
            {
                var mock = new Mock<IDbConnection>();
                return mock.Object;
            }
        }
    }
}