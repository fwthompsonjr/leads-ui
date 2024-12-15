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
    public class UserUsageRepositoryTests
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
        public async Task RepoCanAppendUsageRecordAsync(int conditionId)
        {
            var dto = conditionId == 0 ? null : appendfaker.Generate();
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            if (conditionId < 0)
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<DbCountyAppendLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<DbCountyAppendLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.AppendUsageRecord(new jdbc.models.UserUsageAppendRecordModel());
            mock.Verify(x => x.QuerySingleOrDefaultAsync<DbCountyAppendLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public async Task RepoCanCompleteUsageRecordAsync(int conditionId)
        {
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            if (conditionId < 0)
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).Returns(Task.CompletedTask);
            }
            _ = await service.CompleteUsageRecord(new());
            mock.Verify(x => x.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }


        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public async Task RepoCanGetMonthlyLimitByIdAsync(int conditionId)
        {
            var dto = conditionId <= 0 ? null : usagefaker.Generate(conditionId);
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            if (conditionId < 0)
            {
                mock.Setup(x => x.QueryAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QueryAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.GetMonthlyLimit("");
            mock.Verify(x => x.QueryAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task RepoCanGetMonthlyLimitByCountyIdAsync(int conditionId)
        {
            var dto = conditionId <= 0 ? null : usagefaker.Generate();
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            if (conditionId < 0)
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.GetMonthlyLimit("", 0);
            mock.Verify(x => x.QuerySingleOrDefaultAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(25, true)]
        public async Task RepoCanGetUsageAsync(int conditionId, bool isMonthlyRequest = false)
        {
            var dto = conditionId < 0 ? null : recordfaker.Generate(conditionId);
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            var leadId = provider.LeadId;
            var searchDate = provider.SearchDate;
            if (conditionId < 0)
            {
                mock.Setup(x => x.QueryAsync<DbCountyUsageRequestDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QueryAsync<DbCountyUsageRequestDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.GetUsage(leadId, searchDate, isMonthlyRequest);
            mock.Verify(x => x.QueryAsync<DbCountyUsageRequestDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }


        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(25, true)]
        public async Task RepoCanGetUsageSummaryAsync(int conditionId, bool isMonthlyRequest = false)
        {
            var dto = conditionId < 0 ? null : summaryfaker.Generate(conditionId);
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            var leadId = provider.LeadId;
            var searchDate = provider.SearchDate;
            if (conditionId < 0)
            {
                mock.Setup(x => x.QueryAsync<DbUsageSummaryDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QueryAsync<DbUsageSummaryDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.GetUsageSummary(leadId, searchDate, isMonthlyRequest);
            mock.Verify(x => x.QueryAsync<DbUsageSummaryDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task RepoCanSetMonthlyLimitAsync(int conditionId)
        {
            var dto = conditionId <= 0 ? null : usagefaker.Generate();
            var provider = new RepoContainer();
            var service = provider.Repository;
            var mock = provider.DbCommandMock;
            var leadId = provider.LeadId;
            var countyId = provider.CountyId;
            var limit = provider.MonthlyLimit;
            if (conditionId < 0)
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(provider.Error);
            }
            else
            {
                mock.Setup(x => x.QuerySingleOrDefaultAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(dto);
            }
            _ = await service.SetMonthlyLimit(leadId, countyId, limit);
            mock.Verify(x => x.QuerySingleOrDefaultAsync<DbCountyUsageLimitDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }
        private sealed class RepoContainer
        {
            private readonly IUserUsageRepository repo;

            public Exception Error { get; private set; }

            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {
                var fkr = new Faker();
                command = new Mock<IDapperCommand>();
                var dataContext = new DataContext(command.Object);
                repo = new UserUsageRepository(dataContext);
                Error = fkr.System.Exception();
                LeadId = fkr.Random.Guid().ToString();
                SearchDate = fkr.Date.Past();
                CountyId = fkr.Random.Int(1, 1000);
                MonthlyLimit = fkr.Random.Int(100, 10000);
            }

            public IUserUsageRepository Repository => repo;
            public Mock<IDapperCommand> DbCommandMock => command;
            public string LeadId { get; private set; }
            public DateTime SearchDate { get; private set; }
            public int CountyId { get; private set; }
            public int MonthlyLimit { get; private set; }
        }

        private static readonly Faker<DbCountyAppendLimitDto> appendfaker =
            new Faker<DbCountyAppendLimitDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"));

        private static readonly Faker<DbCountyUsageLimitDto> usagefaker
            = new Faker<DbCountyUsageLimitDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.LeadUserId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CountyId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.MaxRecords, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.IsActive, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.CompleteDate, y => y.Date.Recent());

        private static readonly Faker<DbCountyUsageRequestDto> recordfaker
            = new Faker<DbCountyUsageRequestDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.LeadUserId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CountyId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.CountyName, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.StartDate, y => y.Date.Recent())
            .RuleFor(x => x.EndDate, y => y.Date.Recent())
            .RuleFor(x => x.DateRange, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.RecordCount, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.CompleteDate, y => y.Date.Recent());

        private static readonly Faker<DbUsageSummaryDto> summaryfaker
            = new Faker<DbUsageSummaryDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.UserName, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.SearchYear, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.SearchMonth, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.LastSearchDate, y => y.Date.Recent())
            .RuleFor(x => x.LeadUserId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CountyId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.CountyName, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.MTD, y => y.Random.Int())
            .RuleFor(x => x.MonthlyLimit, y => y.Random.Int(1, 555555));
    }
}