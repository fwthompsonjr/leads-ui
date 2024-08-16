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
    public class AppSettingRepositoryTests
    {
        private static readonly Faker<AppSettingDto> dtofaker =
            new Faker<AppSettingDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyValue, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Version, y => y.Random.Decimal(0, 2))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

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
        [InlineData(false, 0)]
        [InlineData(false, 5)]
        [InlineData(false, 10)]
        [InlineData(true, 10)]
        public void RepoCanBegin(bool hasException, int recordCount)
        {
            var request = faker.Random.AlphaNumeric(10);
            var exception = faker.System.Exception();
            var container = new RepoContainer();
            var response = dtofaker.Generate(recordCount);
            var service = container.Repo;
            var mock = container.CommandMock;
            if (hasException)
            {
                mock.Setup(m => m.QueryAsync<AppSettingDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(exception);
            }
            else
            {
                mock.Setup(m => m.QueryAsync<AppSettingDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(response);
            }
            _ = service.FindKey(request);
            mock.Verify(m => m.QueryAsync<AppSettingDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        private sealed class RepoContainer
        {
            private readonly AppSettingRepository repo;
            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {
                command = new Mock<IDapperCommand>();
                var dataContext = new MockDataContext(command.Object);
                repo = new AppSettingRepository(dataContext);
            }

            public AppSettingRepository Repo => repo;
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