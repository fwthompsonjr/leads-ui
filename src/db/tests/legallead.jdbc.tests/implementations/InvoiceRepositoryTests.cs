using Bogus;
using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;
using System.Data;

namespace legallead.jdbc.tests.implementations
{
    public class InvoiceRepositoryTests
    {

        [Fact]
        public void RepoCanBeConstructed()
        {
            var provider = new RepoContainer();
            var repo = provider.Repository;
            Assert.NotNull(repo);
        }


        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public async Task RepoCanCreateAccountAsync(int conditionId)
        {
            const string proc = "CALL USP_LEADUSER_CREATE_CUSTOMER_ACCOUNT ( ? );";
            var dto = conditionId < 0 ? null : accountfaker.Generate(conditionId);
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            if (conditionId < 0)
            {
                mock.Setup(x => x.QueryAsync<LeadCustomerDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(proc)),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QueryAsync<LeadCustomerDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(proc)),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.CreateAccountAsync(new());
            mock.Verify(x => x.QueryAsync<LeadCustomerDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(proc)),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public async Task RepoCanFindAllAsync(int conditionId)
        {
            const string proc = "CALL USP_LEADUSER_LIST_INVOICES ( );";
            var dto = conditionId < 0 ? null : recordfaker.Generate(conditionId);
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            if (conditionId < 0)
            {
                mock.Setup(x => x.QueryAsync<DbInvoiceViewDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(proc)),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QueryAsync<DbInvoiceViewDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(proc)),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.FindAllAsync();
            mock.Verify(x => x.QueryAsync<DbInvoiceViewDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(proc)),
                    It.IsAny<DynamicParameters>()));
        }


        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public async Task RepoCanFindAccountAsync(int conditionId)
        {
            const string proc = "CALL USP_LEADUSER_FIND_CUSTOMER_ACCOUNT ( ? );";
            var dto = conditionId < 0 ? null : accountfaker.Generate(conditionId);
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            if (conditionId < 0)
            {
                mock.Setup(x => x.QueryAsync<LeadCustomerDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(proc)),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QueryAsync<LeadCustomerDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(proc)),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.FindAccountAsync(new());
            mock.Verify(x => x.QueryAsync<LeadCustomerDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(proc)),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task RepoCanGenerateInvoicesAsync(int conditionId)
        {
            const string proc = "CALL USP_LEADUSER_GENERATE_INVOICE ( );";
            var dto = conditionId == 0 ? null : generatefaker.Generate();
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            if (conditionId < 0)
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<GenerateInvoiceDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(proc)),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<GenerateInvoiceDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(proc)),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.GenerateInvoicesAsync();
            mock.Verify(x => x.QuerySingleOrDefaultAsync<GenerateInvoiceDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(proc)),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public async Task RepoCanQueryAsync(int conditionId)
        {
            const string proc = "CALL USP_LEADUSER_QUERY_INVOICES ( ? );";
            var dto = conditionId < 0 ? null : recordfaker.Generate(conditionId);
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            if (conditionId < 0)
            {
                mock.Setup(x => x.QueryAsync<DbInvoiceViewDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(proc)),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QueryAsync<DbInvoiceViewDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(proc)),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.QueryAsync(new());
            mock.Verify(x => x.QueryAsync<DbInvoiceViewDto>(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(proc)),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task RepoCanUpdateAsync(int conditionId)
        {
            const string proc = "CALL USP_LEADUSER_UPDATE_INVOICE ( ? );";
            var query = new DbInvoiceViewBo();
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            if (conditionId != 0) query.Id = "testing";
            if (conditionId < 0)
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(proc)),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.Is<string>(s => s.Equals(proc)),
                    It.IsAny<DynamicParameters>())).Returns(Task.CompletedTask);
            }
            var response = await service.UpdateAsync(query);
            var expected = conditionId == 1;
            Assert.Equal(expected, response.Key);
            if (conditionId != 0) 
                mock.Verify(x => x.ExecuteAsync(
                        It.IsAny<IDbConnection>(),
                        It.Is<string>(s => s.Equals(proc)),
                        It.IsAny<DynamicParameters>()));
            else
                mock.Verify(x => x.ExecuteAsync(
                        It.IsAny<IDbConnection>(),
                        It.Is<string>(s => s.Equals(proc)),
                        It.IsAny<DynamicParameters>()), Times.Never);
        }

        private sealed class RepoContainer
        {
            private readonly IInvoiceRepository repo;

            public Exception Error { get; private set; }

            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {
                var fkr = new Faker();
                command = new Mock<IDapperCommand>();
                var dataContext = new DataContext(command.Object);
                repo = new InvoiceRepository(dataContext);
                Error = fkr.System.Exception();
                LeadId = fkr.Random.Guid().ToString();
                SearchDate = fkr.Date.Past();
                CountyId = fkr.Random.Int(1, 1000);
                MonthlyLimit = fkr.Random.Int(100, 10000);
            }

            public IInvoiceRepository Repository => repo;
            public Mock<IDapperCommand> DbCommandMock => command;
            public string LeadId { get; private set; }
            public DateTime SearchDate { get; private set; }
            public int CountyId { get; private set; }
            public int MonthlyLimit { get; private set; }
        }

        private static readonly Faker<GenerateInvoiceDto> generatefaker =
            new Faker<GenerateInvoiceDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.RecordCount, y => y.Random.Int(1, 555555));

        private static readonly Faker<DbInvoiceViewDto> recordfaker
            = new Faker<DbInvoiceViewDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.UserName, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.Email, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.RequestId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.LeadUserId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.InvoiceNbr, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.InvoiceUri, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.RecordCount, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.Description, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.ItemCount, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.ItemPrice, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.ItemTotal, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.InvoiceTotal, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.CompleteDate, y => y.Date.Recent());

        private static readonly Faker<LeadCustomerDto> accountfaker
            = new Faker<LeadCustomerDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.LeadUserId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CustomerId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.Email, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.IsTest, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());
    }
}