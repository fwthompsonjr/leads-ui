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
    public class SearchRepositoryTests
    {
        [Fact]
        public void RepoCanBeConstructed()
        {
            var provider = new SearchRepoContainer();
            var repo = provider.SearchRepo;
            Assert.NotNull(repo);
        }

        [Fact]
        public async Task RepoCanAppend()
        {
            var completion = Task.CompletedTask;
            var provider = new SearchRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.SearchRepo;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .Returns(completion);
            var response = await service.Append(SearchTargetTypes.Detail, "", "");
            Assert.True(response.Key);
        }

        [Theory]
        [InlineData(SearchTargetTypes.Detail)]
        [InlineData(SearchTargetTypes.Request)]
        [InlineData(SearchTargetTypes.Response)]
        [InlineData(SearchTargetTypes.Status)]
        [InlineData(SearchTargetTypes.Staging)]
        public async Task RepoCanAppendAllTypes(SearchTargetTypes type)
        {
            var completion = Task.CompletedTask;
            var provider = new SearchRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.SearchRepo;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .Returns(completion);
            var response = await service.Append(type, "", "");
            Assert.True(response.Key);
        }

        [Fact]
        public async Task RepoCanAppendStagingBytes()
        {
            var type = SearchTargetTypes.Staging;
            var completion = Task.CompletedTask;
            var provider = new SearchRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.SearchRepo;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .Returns(completion);
            var response = await service.Append(type, "", Encoding.UTF8.GetBytes("message"));
            Assert.True(response.Key);
        }

        [Fact]
        public async Task RepoCanBeginHappyPath()
        {
            var completion = new SearchDto { Id = Guid.NewGuid().ToString("D") };
            var provider = new SearchRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.SearchRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<SearchDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.Begin("", "");
            Assert.True(response.Key);
        }

        [Fact]
        public async Task RepoCanBeginEmptyId()
        {
            var completion = new SearchDto { Id = string.Empty };
            var provider = new SearchRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.SearchRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<SearchDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.Begin("", "");
            Assert.False(response.Key);
        }

        [Fact]
        public async Task RepoCanBeginNoResponse()
        {
            SearchDto? completion = default;
            var provider = new SearchRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.SearchRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<SearchDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.Begin("", "");
            Assert.False(response.Key);
        }

        [Fact]
        public async Task RepoCanBeginExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new SearchRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.SearchRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<SearchDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.Begin("", "");
            Assert.False(response.Key);
        }

        [Fact]
        public async Task RepoCanCompleteHappyPath()
        {
            var completion = Task.CompletedTask;
            var provider = new SearchRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.SearchRepo;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .Returns(completion);
            var response = await service.Complete("");
            Assert.True(response.Key);
        }

        [Fact]
        public async Task RepoCanCompleteExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new SearchRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.SearchRepo;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.Complete("");
            Assert.False(response.Key);
        }

        [Fact]
        public async Task RepoCanUpdateRowCountHappyPath()
        {
            var completion = Task.CompletedTask;
            var provider = new SearchRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.SearchRepo;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .Returns(completion);
            var response = await service.UpdateRowCount("", 5);
            Assert.True(response.Key);
        }

        [Fact]
        public async Task RepoCanUpdateRowCountExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new SearchRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.SearchRepo;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.UpdateRowCount("", 2);
            Assert.False(response.Key);
        }

        private sealed class SearchRepoContainer
        {
            private readonly IUserSearchRepository repo;
            private readonly Mock<IDapperCommand> command;
            public SearchRepoContainer()
            {

                command = new Mock<IDapperCommand>();
                var dataContext = new DataContext(command.Object);
                repo = new UserSearchRepository(dataContext);
            }

            public IUserSearchRepository SearchRepo => repo;
            public Mock<IDapperCommand> CommandMock => command;
        }
    }
}
