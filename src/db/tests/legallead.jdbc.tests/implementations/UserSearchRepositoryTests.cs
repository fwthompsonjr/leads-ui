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