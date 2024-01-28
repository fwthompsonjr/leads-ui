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
    public class UserPermissionHistoryRepositoryTests
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
        public async Task RepoCreateSnapshotHappyPath()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var change = faker.PickRandom<PermissionChangeTypes>();
                var container = new RepoContainer();
                var service = container.Repo;
                await service.CreateSnapshot(new User(), change);
            });
            Assert.Null(exception);
        }

        [Fact]
        public async Task RepoGetAllNoResult()
        {
            UserPermissionHistory[] result = Array.Empty<UserPermissionHistory>();

            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<UserPermissionHistory>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            await service.GetAll(new User());
            mock.Verify(m => m.QueryAsync<UserPermissionHistory>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }

        [Fact]
        public async Task RepoGetAllMultipleResult()
        {
            UserPermissionHistory[] result = new[] {
                new UserPermissionHistory { GroupId = 1, KeyName = "A" },
                new UserPermissionHistory { GroupId = 1, KeyName = "B" },
                new UserPermissionHistory { GroupId = 1, KeyName = "C" },
                new UserPermissionHistory { GroupId = 2, KeyName = "A" },
                new UserPermissionHistory { GroupId = 2, KeyName = "B" },
            };
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<UserPermissionHistory>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            await service.GetAll(new User());
            mock.Verify(m => m.QueryAsync<UserPermissionHistory>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }

        [Fact]
        public async Task RepoGetLatestMultipleResult()
        {
            UserPermissionHistory[] result = new[] {
                new UserPermissionHistory { GroupId = 1, KeyName = "A" },
                new UserPermissionHistory { GroupId = 1, KeyName = "B" },
                new UserPermissionHistory { GroupId = 1, KeyName = "C" },
                new UserPermissionHistory { GroupId = 2, KeyName = "A" },
                new UserPermissionHistory { GroupId = 2, KeyName = "B" },
            };
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            mock.Setup(m => m.QueryAsync<UserPermissionHistory>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            )).ReturnsAsync(result);
            await service.GetLatest(new User());
            mock.Verify(m => m.QueryAsync<UserPermissionHistory>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()
            ));
        }

        private sealed class RepoContainer
        {
            private readonly UserPermissionHistoryRepository repo;
            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {
                command = new Mock<IDapperCommand>();
                var dataContext = new MockDataContext(command.Object);
                repo = new UserPermissionHistoryRepository(dataContext);
            }

            public UserPermissionHistoryRepository Repo => repo;
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