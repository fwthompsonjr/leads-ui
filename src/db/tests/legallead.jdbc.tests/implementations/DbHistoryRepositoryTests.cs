using Bogus;
using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using Moq;
using System.Data;

namespace legallead.jdbc.tests.implementations
{
    public class DbHistoryRepositoryTests
    {

        private static readonly Faker<DbSearchHistoryDto> faker
            = new Faker<DbSearchHistoryDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        private static readonly Faker<DbSearchHistoryResultDto> rfaker
            = new Faker<DbSearchHistoryResultDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void RepoCanBeConstructed()
        {
            var provider = new RepoContainer();
            var repo = provider.Repository;
            Assert.NotNull(repo);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task RepoCanBeginAsync(int recordcount)
        {
            var error = new Faker().System.Exception();
            var request = new DbHistoryRequest();
            var completion = recordcount switch
            {
                -1 => null,
                _ => faker.Generate()
            };
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            if (recordcount == 0)
            {
                mock.Setup(m => m.QuerySingleOrDefaultAsync<DbSearchHistoryDto>(
                    It.IsAny<IDbConnection>(),
                        It.IsAny<string>(),
                        It.IsAny<DynamicParameters>()))
                        .ThrowsAsync(error);
            }
            else
            {
                mock.Setup(m => m.QuerySingleOrDefaultAsync<DbSearchHistoryDto>(
                    It.IsAny<IDbConnection>(),
                        It.IsAny<string>(),
                        It.IsAny<DynamicParameters>()))
                        .ReturnsAsync(completion);
            }
            _ = await service.BeginAsync(request);

            mock.Verify(m => m.QuerySingleOrDefaultAsync<DbSearchHistoryDto>(
                It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task RepoCanCompleteAsync(int recordcount)
        {
            var error = new Faker().System.Exception();
            var request = new DbHistoryRequest();
            var completion = recordcount switch
            {
                -1 => null,
                _ => faker.Generate()
            };
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            if (recordcount == 0)
            {
                mock.Setup(m => m.QuerySingleOrDefaultAsync<DbSearchHistoryDto>(
                    It.IsAny<IDbConnection>(),
                        It.IsAny<string>(),
                        It.IsAny<DynamicParameters>()))
                        .ThrowsAsync(error);
            }
            else
            {
                mock.Setup(m => m.QuerySingleOrDefaultAsync<DbSearchHistoryDto>(
                    It.IsAny<IDbConnection>(),
                        It.IsAny<string>(),
                        It.IsAny<DynamicParameters>()))
                        .ReturnsAsync(completion);
            }
            _ = await service.CompleteAsync(request);

            mock.Verify(m => m.QuerySingleOrDefaultAsync<DbSearchHistoryDto>(
                It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-100)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(50)]
        public async Task RepoCanFindAsync(int recordcount)
        {
            var fkr = new Faker();
            var error = fkr.System.Exception();
            var request = fkr.Random.Guid().ToString();
            var completion = recordcount switch
            {
                -1 => null,
                -100 => rfaker.Generate(1),
                _ => rfaker.Generate(recordcount)
            };
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            if (recordcount == -100)
            {
                mock.Setup(m => m.QueryAsync<DbSearchHistoryResultDto>(
                    It.IsAny<IDbConnection>(),
                        It.IsAny<string>(),
                        It.IsAny<DynamicParameters>()))
                        .ThrowsAsync(error);
            }
            else
            {
                mock.Setup(m => m.QueryAsync<DbSearchHistoryResultDto>(
                    It.IsAny<IDbConnection>(),
                        It.IsAny<string>(),
                        It.IsAny<DynamicParameters>()))
                        .ReturnsAsync(completion);
            }
            _ = await service.FindAsync(request);

            mock.Verify(m => m.QueryAsync<DbSearchHistoryResultDto>(
                It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task RepoCanUploadAsync(int recordcount)
        {
            var error = new Faker().System.Exception();
            var request = new DbUploadRequest();
            var completion = Task.CompletedTask;
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            if (recordcount == 0)
            {
                mock.Setup(m => m.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                        It.IsAny<string>(),
                        It.IsAny<DynamicParameters>()))
                        .ThrowsAsync(error);
            }
            else
            {
                mock.Setup(m => m.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                        It.IsAny<string>(),
                        It.IsAny<DynamicParameters>()))
                        .Returns(completion);
            }
            _ = await service.UploadAsync(request);

            mock.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        private sealed class RepoContainer
        {
            private readonly IDbHistoryRepository repo;
            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {

                command = new Mock<IDapperCommand>();
                var dataContext = new DataContext(command.Object);
                repo = new DbHistoryRepository(dataContext);
            }

            public IDbHistoryRepository Repository => repo;
            public Mock<IDapperCommand> CommandMock => command;
        }
    }
}