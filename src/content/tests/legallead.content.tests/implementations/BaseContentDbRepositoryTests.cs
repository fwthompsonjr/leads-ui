using Bogus;
using Dapper;
using legallead.content.helpers;
using legallead.content.interfaces;
using legallead.content.tests.testobj;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Data;

namespace legallead.content.tests.implementations
{
    public class BaseContentDbRepositoryTests
    {
        [Theory]
        [InlineData(typeof(ContentDbContext))]
        [InlineData(typeof(IContentDbCommand))]
        [InlineData(typeof(IDbConnection))]
        [InlineData(typeof(IDbCommand))]
        [InlineData(typeof(TempDto))]
        [InlineData(typeof(Faker<TempDto>))]
        [InlineData(typeof(TempDtoRepository))]
        public void ProviderCanCreateObjects(Type type)
        {
            var provider = GetTestFramework();
            var actual = provider.GetService(type);
            Assert.NotNull(actual);
        }

        [Fact]
        public async Task RepositoryCanGetAll()
        {
            int count = new Faker().Random.Int(1, 20);
            var provider = GetTestFramework();
            var repo = provider.GetRequiredService<TempDtoRepository>();
            var faker = provider.GetRequiredService<Faker<TempDto>>();
            var readerMq = provider.GetRequiredService<Mock<IContentDbCommand>>();
            var expected = faker.Generate(count);
            readerMq.Setup(m => m.QueryAsync<TempDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.Is<DynamicParameters?>(a => a == null)))
                .ReturnsAsync(expected);

            var actual = await repo.GetAll();

            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.Equal(count, actual.Count());
        }

        [Fact]
        public async Task RepositoryCanGetById()
        {
            var fkr = new Faker();
            int count = fkr.Random.Int(2, 20);
            int index = fkr.Random.Int(0, count - 1);
            var provider = GetTestFramework();
            var repo = provider.GetRequiredService<TempDtoRepository>();
            var faker = provider.GetRequiredService<Faker<TempDto>>();
            var readerMq = provider.GetRequiredService<Mock<IContentDbCommand>>();
            var collection = faker.Generate(count);
            var expected = collection[index];
            readerMq.Setup(m => m.QuerySingleOrDefaultAsync<TempDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()))
                .ReturnsAsync(expected);

            var actual = await repo.GetById(expected.Id);

            Assert.NotNull(actual);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Fact]
        public async Task RepositoryCanCreate()
        {
            var provider = GetTestFramework();
            var repo = provider.GetRequiredService<TempDtoRepository>();
            var readerMq = provider.GetRequiredService<Mock<IContentDbCommand>>();
            var dto = provider.GetRequiredService<TempDto>();
            readerMq.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));

            await repo.Create(dto);

            readerMq.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Fact]
        public async Task RepositoryCanUpdate()
        {
            var provider = GetTestFramework();
            var repo = provider.GetRequiredService<TempDtoRepository>();
            var readerMq = provider.GetRequiredService<Mock<IContentDbCommand>>();
            var dto = provider.GetRequiredService<TempDto>();
            readerMq.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));

            await repo.Update(dto);

            readerMq.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        [Fact]
        public async Task RepositoryCanDelete()
        {
            var provider = GetTestFramework();
            var repo = provider.GetRequiredService<TempDtoRepository>();
            var readerMq = provider.GetRequiredService<Mock<IContentDbCommand>>();
            var dto = provider.GetRequiredService<TempDto>();
            readerMq.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));

            await repo.Delete(dto.Id);

            readerMq.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>()));
        }

        private static IServiceProvider GetTestFramework()
        {
            return TestContextProvider.GetTestFramework();
        }
    }
}