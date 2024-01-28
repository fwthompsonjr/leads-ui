using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;
using System.Data;

namespace legallead.jdbc.tests.implementations
{
    public class UserTokenRepositoryTests
    {
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
        public async Task RepoGetAll()
        {
            var resultset = Array.Empty<UserRefreshToken>();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<UserRefreshToken>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).ReturnsAsync(resultset);
            _ = await service.GetAll(new User());
            mock.Verify(m => m.QueryAsync<UserRefreshToken>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Fact]
        public async Task RepoCanAdd()
        {
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
            _ = await service.Add(new UserRefreshToken());
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }
        [Fact]
        public async Task RepoCanFind()
        {
            var resultset = new UserRefreshToken();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<UserRefreshToken>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).ReturnsAsync(resultset);
            _ = await service.Find("abc", "123");
            mock.Verify(m => m.QuerySingleOrDefaultAsync<UserRefreshToken>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Fact]
        public async Task RepoCanDeleteTokensWithoutResult()
        {
            var resultset = Array.Empty<UserRefreshToken>();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<UserRefreshToken>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).ReturnsAsync(resultset);

            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));

            await service.DeleteTokens(new User());
            mock.Verify(m => m.QueryAsync<UserRefreshToken>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));

            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()), Times.Never);
        }

        [Fact]
        public async Task RepoCanDeleteTokensWithoutId()
        {
            var resultset = new[] { new UserRefreshToken() };
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<UserRefreshToken>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).ReturnsAsync(resultset);

            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));

            await service.DeleteTokens(new User());
            mock.Verify(m => m.QueryAsync<UserRefreshToken>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));

            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()), Times.Never);
        }
        [Fact]
        public async Task RepoCanDeleteTokens()
        {
            var resultset = new[] { new UserRefreshToken { Id = Guid.Empty.ToString() } };
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<UserRefreshToken>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).ReturnsAsync(resultset);

            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));

            await service.DeleteTokens(new User());
            mock.Verify(m => m.QueryAsync<UserRefreshToken>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));

            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()), Times.Once);
        }

        private sealed class RepoContainer
        {
            private readonly UserTokenRepository repo;
            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {
                command = new Mock<IDapperCommand>();
                var dataContext = new MockDataContext(command.Object);
                repo = new UserTokenRepository(dataContext);
            }

            public UserTokenRepository Repo => repo;
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