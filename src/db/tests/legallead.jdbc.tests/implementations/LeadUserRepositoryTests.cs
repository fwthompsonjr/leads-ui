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
    public class LeadUserRepositoryTests
    {
        private static readonly Faker<LeadUserDto> userfaker =
            new Faker<LeadUserDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.Phrase, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Vector, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Token, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        private static readonly Faker<LeadUserCountyDto> tokenfaker =
            new Faker<LeadUserCountyDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LeadUserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Phrase, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Vector, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Token, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        private static readonly Faker<LeadUserCountyIndexDto> permissionfaker =
            new Faker<LeadUserCountyIndexDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LeadUserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyList, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

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


        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(4, true)]
        [InlineData(5, true)]
        [InlineData(6, true)]
        public async Task RepoCanAddAccount(int conditionId, bool expected)
        {
            var request = userfaker.Generate();
            var result = conditionId == 5 ? null : userfaker.Generate();
            if (conditionId == 1) { request.UserName = string.Empty; }
            if (conditionId == 2) { request.Phrase = string.Empty; }
            if (conditionId == 3) { request.Vector = string.Empty; }
            if (conditionId == 4) { request.Token = string.Empty; }
            if (conditionId == 6 && result != null) { result.Id = string.Empty; }
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<LeadUserDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            var data = await service.AddAccount(request);
            var actual = string.IsNullOrEmpty(data);
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData(0, true)]
        [InlineData(1, false)]
        [InlineData(2, false)]
        [InlineData(3, false)]
        [InlineData(4, false)]
        [InlineData(5, false)]
        [InlineData(6, false)]
        [InlineData(7, false)]
        public async Task RepoCanAddCountyToken(int conditionId, bool expected)
        {
            var request = tokenfaker.Generate();
            var result = conditionId == 6 ? null : tokenfaker.Generate();
            if (conditionId == 1) { request.LeadUserId = string.Empty; }
            if (conditionId == 2) { request.CountyName = string.Empty; }
            if (conditionId == 3) { request.Phrase = string.Empty; }
            if (conditionId == 4) { request.Vector = string.Empty; }
            if (conditionId == 5) { request.Token = string.Empty; }
            if (conditionId == 7 && result != null) { result.Id = string.Empty; }
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<LeadUserCountyDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            var actual = await service.AddCountyToken(request);
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task RepoCanAddCountyPermissions(int conditionId)
        {
            bool expected = conditionId == 0;
            var request = permissionfaker.Generate();
            var result = conditionId == 3 ? null : permissionfaker.Generate();
            if (conditionId == 1) { request.LeadUserId = string.Empty; }
            if (conditionId == 2) { request.CountyList = string.Empty; }
            if (conditionId == 4 && result != null) { result.Id = string.Empty; }
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<LeadUserCountyIndexDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            var actual = await service.AddCountyPermissions(request);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task RepoCanGetUserGetId(int conditionId)
        {
            var expected = conditionId == 1;
            var request = userfaker.Generate().UserName ?? "test.account";
            var account = conditionId == 1 ? null : userfaker.Generate();
            var countyTokens = conditionId == 2 ? [] : tokenfaker.Generate(2);
            var countyIndexes = conditionId == 3 ? [] : permissionfaker.Generate(2);

            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<LeadUserDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(account);
            mock.Setup(m => m.QueryAsync<LeadUserCountyDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(countyTokens);
            mock.Setup(m => m.QueryAsync<LeadUserCountyIndexDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(countyIndexes);
            var response = await service.GetUserById(request);
            var actual = response == null;
            Assert.Equal(expected, actual);
            if (response != null) Assert.False(string.IsNullOrEmpty(response.UserData));
            if (conditionId != 2 && response != null) Assert.False(string.IsNullOrEmpty(response.CountyData));
            if (conditionId != 3 && response != null) Assert.False(string.IsNullOrEmpty(response.IndexData));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task RepoCanGetUser(int conditionId)
        {
            var expected = conditionId == 1;
            var request = userfaker.Generate().UserName ?? "test.account";
            var account = conditionId == 1 ? null : userfaker.Generate();
            var countyTokens = conditionId == 2 ? [] : tokenfaker.Generate(2);
            var countyIndexes = conditionId == 3 ? [] : permissionfaker.Generate(2);

            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<LeadUserDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(account);
            mock.Setup(m => m.QueryAsync<LeadUserCountyDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(countyTokens);
            mock.Setup(m => m.QueryAsync<LeadUserCountyIndexDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(countyIndexes);
            var response = await service.GetUser(request);
            var actual = response == null;
            Assert.Equal(expected, actual);
            if (response != null) Assert.False(string.IsNullOrEmpty(response.UserData));
            if (conditionId != 2 && response != null) Assert.False(string.IsNullOrEmpty(response.CountyData));
            if (conditionId != 3 && response != null) Assert.False(string.IsNullOrEmpty(response.IndexData));
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, false)]
        [InlineData(2, false)]
        [InlineData(3, false)]
        [InlineData(4, false)]
        [InlineData(5, false)]
        [InlineData(6, false)]
        public async Task RepoCanUpdateAccount(int conditionId, bool expected)
        {
            var request = userfaker.Generate();
            var result = conditionId == 5 ? null : userfaker.Generate();
            if (conditionId == 1) { request.Id = string.Empty; }
            if (conditionId == 2) { request.Phrase = string.Empty; }
            if (conditionId == 3) { request.Vector = string.Empty; }
            if (conditionId == 4) { request.Token = string.Empty; }
            if (conditionId == 6 && result != null) { result.Id = string.Empty; }
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<LeadUserDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            var actual = await service.UpdateAccount(request);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, false)]
        [InlineData(2, false)]
        [InlineData(3, false)]
        [InlineData(4, false)]
        [InlineData(5, false)]
        [InlineData(6, false)]
        [InlineData(7, false)]
        public async Task RepoCanUpdateCountyToken(int conditionId, bool expected)
        {
            var request = tokenfaker.Generate();
            var result = conditionId == 6 ? null : tokenfaker.Generate();
            if (conditionId == 1) { request.LeadUserId = string.Empty; }
            if (conditionId == 2) { request.CountyName = string.Empty; }
            if (conditionId == 3) { request.Phrase = string.Empty; }
            if (conditionId == 4) { request.Vector = string.Empty; }
            if (conditionId == 5) { request.Token = string.Empty; }
            if (conditionId == 7 && result != null) { result.Id = string.Empty; }
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<LeadUserCountyDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            var actual = await service.UpdateCountyToken(request);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task RepoCanUpdateCountyPermissions(int conditionId)
        {
            bool expected = conditionId == 0;
            var request = permissionfaker.Generate();
            var result = conditionId == 3 ? null : permissionfaker.Generate();
            if (conditionId == 1) { request.LeadUserId = string.Empty; }
            if (conditionId == 2) { request.CountyList = string.Empty; }
            if (conditionId == 4 && result != null) { result.Id = string.Empty; }
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<LeadUserCountyIndexDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            var actual = await service.UpdateCountyPermissions(request);
            Assert.Equal(expected, actual);
        }

        private sealed class RepoContainer
        {
            private readonly LeadUserRepository repo;
            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {
                command = new Mock<IDapperCommand>();
                var dataContext = new MockDataContext(command.Object);
                repo = new LeadUserRepository(dataContext);
            }

            public LeadUserRepository Repo => repo;
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