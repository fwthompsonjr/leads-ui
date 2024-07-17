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
    public class SearchStatusRepositoryTests
    {

        private static readonly Faker<WorkIndexBo> idfaker =
            new Faker<WorkIndexBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"));

        private static readonly Faker<WorkBeginningBo> beginfaker =
            new Faker<WorkBeginningBo>()
            .RuleFor(x => x.WorkIndexes, y =>
            {
                var n = y.Random.Int(1, 8);
                return idfaker.Generate(n);
            });
        private static readonly Faker<WorkStatusBo> statusfaker =
            new Faker<WorkStatusBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.MessageId, y => y.Random.Int(0, 6))
            .RuleFor(x => x.StatusId, y => y.Random.Int(0, 2));

        private static readonly Faker<WorkingSearchDto> dtofaker =
            new Faker<WorkingSearchDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.MessageId, y => y.Random.Int(0, 6))
            .RuleFor(x => x.StatusId, y => y.Random.Int(0, 2))
            .RuleFor(x => x.MachineName, y => y.Hacker.Phrase())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.LastUpdateDt, y => y.Date.Recent())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent());

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

        [Theory]
        [InlineData(false, 5)]
        [InlineData(false, 10)]
        [InlineData(true, 10)]
        public void RepoCanBegin(bool hasException, int recordCount)
        {
            var request = beginfaker.Generate(recordCount);
            var exception = faker.System.Exception();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            if (hasException)
            {
                mock.Setup(m => m.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(exception);
            }
            else
            {
                mock.Setup(m => m.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).Verifiable();
            }
            _ = service.Begin(request[0]);
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }
        [Theory]
        [InlineData(false, 5)]
        [InlineData(false, 10)]
        [InlineData(true, 10)]
        public void RepoCanUpdate(bool hasException, int recordCount)
        {
            var request = statusfaker.Generate(recordCount);
            var exception = faker.System.Exception();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            if (hasException)
            {
                mock.Setup(m => m.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(exception);
            }
            else
            {
                mock.Setup(m => m.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).Verifiable();
            }
            _ = service.Update(request[0]);
            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(false, 0)]
        [InlineData(false, 5)]
        [InlineData(false, 10)]
        [InlineData(true, 10)]
        public void RepoCanList(bool hasException, int recordCount)
        {
            var response = recordCount switch
            {
                0 => null,
                _ => dtofaker.Generate(recordCount)
            };
            var exception = faker.System.Exception();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            if (hasException)
            {
                mock.Setup(m => m.QueryAsync<WorkingSearchDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(exception);
            }
            else
            {
                mock.Setup(m => m.QueryAsync<WorkingSearchDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(response);
            }
            _ = service.List();
            mock.Verify(m => m.QueryAsync<WorkingSearchDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }
        private sealed class RepoContainer
        {
            private readonly SearchStatusRepository repo;
            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {
                command = new Mock<IDapperCommand>();
                var dataContext = new MockDataContext(command.Object);
                repo = new SearchStatusRepository(dataContext);
            }

            public SearchStatusRepository Repo => repo;
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