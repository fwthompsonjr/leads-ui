using Bogus;
using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.enumerations;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;
using System.Data;
using System.Text;

namespace legallead.jdbc.tests.implementations
{
    public class QueueWorkRepositoryTests
    {
        private static readonly Faker<QueueWorkingBo> bofaker =
            new Faker<QueueWorkingBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Message, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StatusId, y => y.Random.Int(-1, 2000))
            .RuleFor(x => x.MachineName, y => y.Random.AlphaNumeric(50))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(300))
            .RuleFor(x => x.LastUpdateDt, y => y.Date.Recent(15))
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent(15));

        private static readonly Faker<QueueWorkingDto> dtofaker =
            new Faker<QueueWorkingDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Message, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StatusId, y => y.Random.Int(-1, 2000))
            .RuleFor(x => x.MachineName, y => y.Random.AlphaNumeric(50))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(300))
            .RuleFor(x => x.LastUpdateDt, y => y.Date.Recent(15))
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent(15));

        private static readonly Faker<CustomerDto> customerfaker =
            new Faker<CustomerDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.Email, y => y.Person.Email);

        private static readonly Faker<StatusSummaryDto> statusfaker =
            new Faker<StatusSummaryDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Region, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Count, y => y.Random.Int(0, 125000))
            .RuleFor(x => x.Oldest, y => y.Date.Recent())
            .RuleFor(x => x.Newest, y => y.Date.Recent());

        private static readonly Faker<StatusDto> summaryfaker =
            new Faker<StatusDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SearchProgress, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Total, y => y.Random.Int(0, 125000));

        private static readonly Faker<QueueNonPersonDto> nonpersonfaker =
            new Faker<QueueNonPersonDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExpectedRows, y => y.Random.Int(0, 750000))
            .RuleFor(x => x.SearchProgress, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StateCode, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.EndDate, y => y.Date.Recent())
            .RuleFor(x => x.StartDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.ExcelData, y =>
            {
                var text = y.Hacker.Phrase();
                return Encoding.UTF8.GetBytes(text);
            });

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
        public void RepoCanInsertRange(bool hasException, int recordCount)
        {
            var request = faker.Random.AlphaNumeric(10);
            var exception = faker.System.Exception();
            var container = new RepoContainer();
            var response = dtofaker.Generate(recordCount);
            var service = container.Repo;
            var mock = container.CommandMock;
            if (hasException)
            {
                mock.Setup(m => m.QueryAsync<QueueWorkingDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(exception);
            }
            else
            {
                mock.Setup(m => m.QueryAsync<QueueWorkingDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(response);
            }
            _ = service.InsertRange(request);
            mock.Verify(m => m.QueryAsync<QueueWorkingDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(false, 0)]
        [InlineData(false, 5)]
        [InlineData(false, 10)]
        [InlineData(true, 10)]
        public void RepoCanUpdateStatus(bool hasException, int recordCount)
        {
            var request = bofaker.Generate();
            var exception = faker.System.Exception();
            var container = new RepoContainer();
            var response = dtofaker.Generate(recordCount);
            var service = container.Repo;
            var mock = container.CommandMock;
            if (hasException)
            {
                mock.Setup(m => m.QueryAsync<QueueWorkingDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(exception);
            }
            else
            {
                mock.Setup(m => m.QueryAsync<QueueWorkingDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(response);
            }
            _ = service.UpdateStatus(request);
            mock.Verify(m => m.QueryAsync<QueueWorkingDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }


        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void RepoCanUpdatePersonData(bool hasException)
        {
            var request = new QueuePersonDataBo
            {
                Id = "123",
                Name = "abc"
            };
            var exception = faker.System.Exception();
            var container = new RepoContainer();
            var service = container.Repo;
            var mock = container.CommandMock;
            if (hasException)
            {
                mock.Setup(m => m.QueryAsync<QueueWorkingDto>(
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
            _ = service.UpdatePersonData(request);
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
        public void RepoCanFetch(bool hasException, int recordCount)
        {
            var exception = faker.System.Exception();
            var container = new RepoContainer();
            var response = dtofaker.Generate(recordCount);
            var service = container.Repo;
            var mock = container.CommandMock;
            if (hasException)
            {
                mock.Setup(m => m.QueryAsync<QueueWorkingDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(exception);
            }
            else
            {
                mock.Setup(m => m.QueryAsync<QueueWorkingDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(response);
            }
            _ = service.Fetch();
            mock.Verify(m => m.QueryAsync<QueueWorkingDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(false, 0)]
        [InlineData(false, 5)]
        [InlineData(false, 10)]
        [InlineData(false, 12)]
        [InlineData(false, 15)]
        [InlineData(true, 10)]
        public void RepoCanGetUserBySearchId(bool hasException, int recordCount)
        {
            var request = bofaker.Generate();
            var exception = faker.System.Exception();
            var container = new RepoContainer();
            var response = customerfaker.Generate(recordCount);
            var service = container.Repo;
            var mock = container.CommandMock;
            if (recordCount == 12) request.Id = string.Empty;
            if (recordCount == 15) request.Id = "not-guid";
            if (hasException)
            {
                mock.Setup(m => m.QueryAsync<CustomerDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(exception);
            }
            else
            {
                mock.Setup(m => m.QueryAsync<CustomerDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(response);
            }
            _ = service.GetUserBySearchId(request.Id);
            if (recordCount > 10)
            {
                mock.Verify(m => m.QueryAsync<CustomerDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()), Times.Never());
                return;
            }
            mock.Verify(m => m.QueryAsync<CustomerDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(false, QueueStatusTypes.Error)]
        [InlineData(false, QueueStatusTypes.Submitted)]
        [InlineData(false, QueueStatusTypes.Purchased)]
        [InlineData(false, QueueStatusTypes.Error, 0)]
        [InlineData(true, QueueStatusTypes.Purchased)]
        public void RepoCanGetSummary(bool hasException, QueueStatusTypes sts, int recordCount = 5)
        {
            var exception = faker.System.Exception();
            var container = new RepoContainer();
            var response = statusfaker.Generate(recordCount);
            var service = container.Repo;
            var mock = container.CommandMock;
            if (hasException)
            {
                mock.Setup(m => m.QueryAsync<StatusSummaryDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(exception);
            }
            else
            {
                mock.Setup(m => m.QueryAsync<StatusSummaryDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(response);
            }
            _ = service.GetSummary(sts);
            mock.Verify(m => m.QueryAsync<StatusSummaryDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }


        [Theory]
        [InlineData(false)]
        [InlineData(false, 0)]
        [InlineData(true)]
        public void RepoCanGetStatus(bool hasException, int recordCount = 5)
        {
            var exception = faker.System.Exception();
            var container = new RepoContainer();
            var response = summaryfaker.Generate(recordCount);
            var service = container.Repo;
            var mock = container.CommandMock;
            if (hasException)
            {
                mock.Setup(m => m.QueryAsync<StatusDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(exception);
            }
            else
            {
                mock.Setup(m => m.QueryAsync<StatusDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(response);
            }
            _ = service.GetStatus();
            mock.Verify(m => m.QueryAsync<StatusDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(false, 0)]
        [InlineData(true)]
        public void RepoCanGetNonPersonData(bool hasException, int recordCount = 5)
        {
            var exception = faker.System.Exception();
            var container = new RepoContainer();
            var response = nonpersonfaker.Generate(recordCount);
            var service = container.Repo;
            var mock = container.CommandMock;
            if (hasException)
            {
                mock.Setup(m => m.QueryAsync<QueueNonPersonDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(exception);
            }
            else
            {
                mock.Setup(m => m.QueryAsync<QueueNonPersonDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(response);
            }
            _ = service.GetNonPersonData();
            mock.Verify(m => m.QueryAsync<QueueNonPersonDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }
        private sealed class RepoContainer
        {
            private readonly QueueWorkRepository repo;
            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {
                command = new Mock<IDapperCommand>();
                var dataContext = new MockDataContext(command.Object);
                repo = new QueueWorkRepository(dataContext);
            }

            public QueueWorkRepository Repo => repo;
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