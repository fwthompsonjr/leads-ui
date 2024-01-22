using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.jdbc.tests.implementations
{
    public class SearchQueueRepositoryTests
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
        public async Task RepoGetQueueHappyPath()
        {
            var results = Array.Empty<SearchQueueDto>();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<SearchQueueDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).ReturnsAsync(results);
            _ = await service.GetQueue();
            mock.Verify(m => m.QueryAsync<SearchQueueDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }


        [Fact]
        public async Task RepoCompleteHappyPath()
        {
            var results = new KeyValuePair<bool, string>(true, "test method");
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.SearchMock;
            mock.Setup(m => m.Complete(It.IsAny<string>())).ReturnsAsync(results);
            _ = await service.Complete(string.Empty);
            mock.Verify(m => m.Complete(It.IsAny<string>()));
        }

        [Fact]
        public async Task RepoStartHappyPath()
        {
            var results = new KeyValuePair<bool, string>(true, "test method");
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.SearchMock;
            mock.Setup(m => m.UpdateRowCount(
                It.IsAny<string>(),
                It.IsAny<int>())).ReturnsAsync(results);
            _ = await service.Start(new SearchDto { Id = "abc123" });
            mock.Verify(m => m.UpdateRowCount(
                It.IsAny<string>(),
                It.IsAny<int>()));
        }

        [Fact]
        public async Task RepoStatusHappyPath()
        {
            var results = new KeyValuePair<bool, string>(true, "test method");
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.SearchMock;
            mock.Setup(m => m.Append(
                It.IsAny<SearchTargetTypes>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<string>())).ReturnsAsync(results);
            _ = await service.Status("abc123", "message");
            mock.Verify(m => m.Append(
                It.IsAny<SearchTargetTypes>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<string>()));
        }

        [Fact]
        public async Task RepoContentHappyPath()
        {
            var results = new KeyValuePair<bool, string>(true, "test method");
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.SearchMock;
            mock.Setup(m => m.Append(
                It.IsAny<SearchTargetTypes>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<string>())).ReturnsAsync(results);
            _ = await service.Content("abc123", Encoding.UTF8.GetBytes("message"));
            mock.Verify(m => m.Append(
                It.IsAny<SearchTargetTypes>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<string>()));
        }

        private sealed class RepoContainer
        {
            private readonly SearchQueueRepository repo;
            private readonly Mock<IDapperCommand> command;
            private readonly Mock<IUserSearchRepository> search;
            public RepoContainer()
            {
                command = new Mock<IDapperCommand>();
                search = new Mock<IUserSearchRepository>();
                var dataContext = new MockDataContext(command.Object);
                repo = new SearchQueueRepository(dataContext, search.Object);
            }

            public SearchQueueRepository Repo => repo;
            public Mock<IDapperCommand> CommandMock => command;
            public Mock<IUserSearchRepository> SearchMock => search;

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
