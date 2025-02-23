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
    public class CountyFileRepositoryTests
    {
        private static readonly Faker<DbCountyFileModel> modelfaker
            = new Faker<DbCountyFileModel>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25));

        [Fact]
        public void RepoCanBeConstructed()
        {
            var provider = new RepoContainer();
            var repo = provider.Repository;
            Assert.NotNull(repo);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(false, 0)]
        [InlineData(false, 10)]
        [InlineData(false, 20)]
        [InlineData(false, 30)]
        [InlineData(false, 40)]
        [InlineData(false, null, 0)]
        [InlineData(false, null, 10)]
        [InlineData(false, null, 20)]
        [InlineData(false, null, 30)]
        [InlineData(false, null, 40)]
        [InlineData(false, null, null, true)]
        public async Task RepoCanGetContentAsync(
            bool canInitialize,
            int? statusId = null,
            int? typeId = null,
            bool hasFetchError = false)
        {
            var response = new DbCountyFileDto
            {
                FileStatusId = statusId,
                FileTypeId = typeId,
            };
            var error = new Faker().System.Exception();
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            var request = modelfaker.Generate();
            if (canInitialize)
            {
                mock.Setup(s => s.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
            }
            else
            {
                mock.Setup(s => s.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(error);
            }
            if (hasFetchError)
            {
                mock.Setup(s => s.QuerySingleOrDefaultAsync<DbCountyFileDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).ThrowsAsync(error);
            }
            else
            {
                mock.Setup(s => s.QuerySingleOrDefaultAsync<DbCountyFileDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(response);
            }
            var err = await Record.ExceptionAsync(async () =>
            {
                await service.GetContentAsync(request);
            });
            Assert.Null(err);
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RepoCanInitializeAsync(
            bool canInitialize)
        {
            var error = new Faker().System.Exception();
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            if (canInitialize)
            {
                mock.Setup(s => s.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
            }
            else
            {
                mock.Setup(s => s.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(error);
            }
            var err = await Record.ExceptionAsync(async () =>
            {
                await service.InitializeAsync();
            });
            Assert.Null(err);
        }
        private sealed class RepoContainer
        {
            private readonly ICountyFileRepository repo;
            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {

                command = new Mock<IDapperCommand>();
                var dataContext = new DataContext(command.Object);
                repo = new CountyFileRepository(dataContext);
            }

            public ICountyFileRepository Repository => repo;
            public Mock<IDapperCommand> CommandMock => command;
        }
    }
}
