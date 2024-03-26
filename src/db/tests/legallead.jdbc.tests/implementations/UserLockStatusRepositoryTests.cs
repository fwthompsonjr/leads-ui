using Bogus;
using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using Moq;
using Newtonsoft.Json;
using System.Data;

namespace legallead.jdbc.tests.implementations
{
    public class UserLockStatusRepositoryTests
    {

        private static readonly Faker<UserLockStatusDto> faker =
            new Faker<UserLockStatusDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.IsLocked, y => y.Random.Bool())
            .RuleFor(x => x.FailedAttemptCount, y => y.Random.Int(1, 14))
            .RuleFor(x => x.LastFailedAttemptDt, y => y.Date.Recent())
            .RuleFor(x => x.FailedAttemptResetDt, y => y.Date.Recent())
            .RuleFor(x => x.MaxFailedAttempts, y => y.Random.Int(15, 25))
            .RuleFor(x => x.CanResetAccount, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void RepoCanBeConstructed()
        {
            var provider = new RepoContainer();
            var repo = provider.Repository;
            Assert.NotNull(repo);
        }


        [Fact]
        public async Task RepoCanAddIncidentHappyPath()
        {
            var completion = Task.CompletedTask;
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .Returns(completion);
            var response = await service.AddIncident(new());
            Assert.True(response.Key);
        }

        [Fact]
        public async Task RepoCanAddIncidentExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.AddIncident(new());
            Assert.False(response.Key);
        }


        [Fact]
        public async Task RepoCanClearSuspensionHappyPath()
        {
            var completion = Task.CompletedTask;
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .Returns(completion);
            var response = await service.ClearSuspension("");
            Assert.True(response.Key);
        }

        [Fact]
        public async Task RepoCanClearSuspensionExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.ClearSuspension("");
            Assert.False(response.Key);
        }

        [Fact]
        public async Task GetStatusHappyPath()
        {
            var completion = faker.Generate();
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<UserLockStatusDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.GetStatus(completion.Id);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task GetStatusNoResponse()
        {
            UserLockStatusDto? completion = default;
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<UserLockStatusDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.GetStatus("");
            Assert.Null(response);
        }

        [Fact]
        public async Task GetStatusExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<UserLockStatusDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.GetStatus("");
            Assert.Null(response);
        }


        [Fact]
        public async Task RepoCanSuspendHappyPath()
        {
            var completion = Task.CompletedTask;
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .Returns(completion);
            var response = await service.Suspend(new());
            Assert.True(response.Key);
        }

        [Fact]
        public async Task RepoCanSuspendExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.Suspend(new());
            Assert.False(response.Key);
        }

        [Fact]
        public async Task RepoCanUnlockHappyPath()
        {
            var completion = Task.CompletedTask;
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .Returns(completion);
            var response = await service.Unlock(new());
            Assert.True(response.Key);
        }

        [Fact]
        public async Task RepoCanUnlockExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.Unlock(new());
            Assert.False(response.Key);
        }

        private sealed class RepoContainer
        {
            private readonly IUserLockStatusRepository repo;
            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {

                command = new Mock<IDapperCommand>();
                var dataContext = new DataContext(command.Object);
                repo = new UserLockStatusRepository(dataContext);
            }

            public IUserLockStatusRepository Repository => repo;
            public Mock<IDapperCommand> CommandMock => command;
        }
    }
}

/*

 */