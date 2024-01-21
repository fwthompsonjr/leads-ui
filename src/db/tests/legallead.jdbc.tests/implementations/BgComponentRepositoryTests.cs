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
    public class BgComponentRepositoryTests
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
        public async Task RepoGetStatusNeedsParameters()
        {
            var container = new RepoContainer();
            var service = container.Repo;
            var response = await service.GetStatus(null, null);
            Assert.False(response);
        }

        [Fact]
        public async Task RepoGetStatusWithEmptyResultReturnsTrue()
        {
            var resultset = Array.Empty<BackgroundComponentStatusDto>();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<BackgroundComponentStatusDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).ReturnsAsync(resultset);

            var response = await service.GetStatus("abc", "123");
            Assert.True(response);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(0, false)]
        [InlineData(2, false)]
        [InlineData(-1, false)]
        [InlineData(1, true)]
        public async Task RepoGetStatusExpectedResult(int? statusId, bool expected)
        {
            var dto = new BackgroundComponentStatusDto { StatusId = statusId };
            var resultset = new[] { dto };
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<BackgroundComponentStatusDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).ReturnsAsync(resultset);

            var response = await service.GetStatus("abc", "123");
            Assert.Equal(expected, response);
        }

        [Fact]
        public async Task RepoReportHealthHappyPath()
        {
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));

            var response = await service.ReportHealth("abc", "123", "xyz");
            Assert.True(response);
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Fact]
        public async Task RepoReportHealthExceptionReturnsFalse()
        {
            var exception = new Faker().System.Exception();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).ThrowsAsync(exception);

            var response = await service.ReportHealth("abc", "123", "xyz");
            Assert.False(response);
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }
        private sealed class RepoContainer
        {
            private readonly BgComponentRepository repo;
            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {
                command = new Mock<IDapperCommand>();
                var dataContext = new MockDataContext(command.Object);
                repo = new BgComponentRepository(dataContext);
            }

            public BgComponentRepository Repo => repo;
            public Mock<IDapperCommand> CommandMock => command;

        }


        [Fact]
        public async Task RepoSetStatusHappyPath()
        {
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));

            var response = await service.SetStatus("abc", "123", "xyz");
            Assert.True(response);
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Fact]
        public async Task RepoSetStatusExceptionReturnsFalse()
        {
            var exception = new Faker().System.Exception();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).ThrowsAsync(exception);

            var response = await service.SetStatus("abc", "123", "xyz");
            Assert.False(response);
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
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
