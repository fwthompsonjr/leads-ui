using Bogus;
using Dapper;
using legallead.content.entities;
using legallead.content.interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Data;

namespace legallead.content.tests.implementations
{
    public class WebContentLineRepositoryTests
    {
        private readonly Faker<WebContentLineDto> faker =
            new Faker<WebContentLineDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ContentId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(10, 20000))
            .RuleFor(x => x.InternalId, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.Content, y => y.Hacker.Phrase());

        private readonly Faker<WebContentDto> contentFaker =
            new Faker<WebContentDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.InternalId, y => y.Random.Int(10, 20000))
            .RuleFor(x => x.VersionId, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.IsActive, y => y.Random.Bool())
            .RuleFor(x => x.IsChild, y => y.Random.Bool())
            .RuleFor(x => x.ContentName, y => y.Company.CompanyName())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(300));

        [Fact]
        public async Task RepoCanGetAll()
        {
            var provider = TestContextProvider.GetTestFramework();
            var repo = provider.GetRequiredService<IWebContentLineRepository>();
            var command = provider.GetRequiredService<Mock<IContentDbCommand>>();
            var expected = faker.Generate(3);
            var parent = contentFaker.Generate();
            command.Setup(m => m.QueryAsync<WebContentLineDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>())).ReturnsAsync(expected);

            _ = await repo.GetAll(parent);

            command.Verify(m => m.QueryAsync<WebContentLineDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>()), Times.Once);
        }

        [Fact]
        public async Task RepoCanGetByInternalId()
        {
            var provider = TestContextProvider.GetTestFramework();
            var repo = provider.GetRequiredService<IWebContentLineRepository>();
            var command = provider.GetRequiredService<Mock<IContentDbCommand>>();
            var expected = faker.Generate(3);
            var parent = contentFaker.Generate();
            command.Setup(m => m.QueryAsync<WebContentLineDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>())).ReturnsAsync(expected);

            _ = await repo.GetByInternalId(parent.InternalId.GetValueOrDefault());

            command.Verify(m => m.QueryAsync<WebContentLineDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>()), Times.Once);
        }

        [Fact]
        public async Task RepoCanNotGetAllWithAnEmptyId()
        {
            var provider = TestContextProvider.GetTestFramework();
            var repo = provider.GetRequiredService<IWebContentLineRepository>();
            var command = provider.GetRequiredService<Mock<IContentDbCommand>>();
            var expected = faker.Generate(3);
            var parent = contentFaker.Generate();
            parent.Id = string.Empty;
            command.Setup(m => m.QueryAsync<WebContentLineDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>())).ReturnsAsync(expected);
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                _ = await repo.GetAll(parent);
            });
        }
    }
}