﻿using Bogus;
using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;
using System.Data;

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