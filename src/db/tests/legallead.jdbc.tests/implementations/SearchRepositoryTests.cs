using Bogus;
using Dapper;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;
using System.Data;

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

        [Fact]
        public async Task RepoCanBeginHappyPath()
        {
            var completion = Task.CompletedTask;
            var provider = new SearchRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.SearchRepo;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .Returns(completion);
            var response = await service.Begin("", "");
            Assert.True(response.Key);
        }

        [Fact]
        public async Task RepoCanBeginExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new SearchRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.SearchRepo;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
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
