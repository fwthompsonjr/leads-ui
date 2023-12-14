using Bogus;
using Dapper;
using legallead.content.entities;
using legallead.content.interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.content.tests.implementations
{
    public class WebContentRepositoryTests
    {
        private readonly Faker<WebContentDto> faker =
            new Faker<WebContentDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.InternalId, y => y.Random.Int(10, 20000))
            .RuleFor(x => x.VersionId, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.IsActive, y => y.Random.Bool())
            .RuleFor(x => x.IsChild, y => y.Random.Bool())
            .RuleFor(x => x.ContentName, y => y.Company.CompanyName())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(300));

        [Fact]
        public async Task RepoCanCreateRevision()
        {
            var provider = TestContextProvider.GetTestFramework();
            var repo = provider.GetRequiredService<IWebContentRepository>();
            var command = provider.GetRequiredService<Mock<IContentDbCommand>>();
            command.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>()));

            await repo.CreateRevision(faker.Generate());

            command.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>()), Times.Once);
        }

        [Fact]
        public async Task RepoCanNotCreateRevisionWithNullId()
        {
            var provider = TestContextProvider.GetTestFramework();
            var repo = provider.GetRequiredService<IWebContentRepository>();
            var command = provider.GetRequiredService<Mock<IContentDbCommand>>();
            var dto = faker.Generate();
            dto.Id = string.Empty;

            command.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>()));

            await repo.CreateRevision(dto);

            command.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>()), Times.Never);
        }

        [Fact]
        public async Task RepoCanGetAllActive()
        {
            var provider = TestContextProvider.GetTestFramework();
            var repo = provider.GetRequiredService<IWebContentRepository>();
            var command = provider.GetRequiredService<Mock<IContentDbCommand>>();
            var expected = faker.Generate(3);
            command.Setup(m => m.QueryAsync<WebContentDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>())).ReturnsAsync(expected);

            _ = await repo.GetAllActive();

            command.Verify(m => m.QueryAsync<WebContentDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>()), Times.Once);
        }

        [Fact]
        public async Task RepoCanGetByInternalId()
        {
            var provider = TestContextProvider.GetTestFramework();
            var repo = provider.GetRequiredService<IWebContentRepository>();
            var command = provider.GetRequiredService<Mock<IContentDbCommand>>();
            var expected = faker.Generate(3);
            command.Setup(m => m.QueryAsync<WebContentDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>())).ReturnsAsync(expected);

            _ = await repo.GetByInternalId(new Faker().Random.Int(1, 1000));

            command.Verify(m => m.QueryAsync<WebContentDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>()), Times.Once);
        }

        [Fact]
        public async Task RepoCanGetByName()
        {
            var provider = TestContextProvider.GetTestFramework();
            var repo = provider.GetRequiredService<IWebContentRepository>();
            var command = provider.GetRequiredService<Mock<IContentDbCommand>>();
            var expected = faker.Generate(3);
            command.Setup(m => m.QueryAsync<WebContentDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>())).ReturnsAsync(expected);

            var fr = new Faker();
            var length = fr.Random.Int(1, 1000);
            var name = fr.Random.AlphaNumeric(length);

            _ = await repo.GetByName(name);

            command.Verify(m => m.QueryAsync<WebContentDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>()), Times.Once);
        }

        [Fact]
        public async Task RepoCanInsert()
        {
            var provider = TestContextProvider.GetTestFramework();
            var repo = provider.GetRequiredService<IWebContentRepository>();
            var command = provider.GetRequiredService<Mock<IContentDbCommand>>();

            command.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>()));

            var fr = new Faker();
            var length = fr.Random.Int(1, 500);
            var name = fr.Random.AlphaNumeric(length);

            await repo.Insert(new CreateContentRequest { Name = name });

            command.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>()), Times.Once);
        }

        [Fact]
        public async Task RepoCanNotInsertAnEmptyName()
        {
            var provider = TestContextProvider.GetTestFramework();
            var repo = provider.GetRequiredService<IWebContentRepository>();
            var command = provider.GetRequiredService<Mock<IContentDbCommand>>();

            command.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>()));

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await repo.Insert(new CreateContentRequest { Name = string.Empty });
            });
        }

        [Theory]
        [InlineData(501)]
        [InlineData(1000)]
        public async Task RepoCanNotInsertAnInvalidName(int length)
        {
            var provider = TestContextProvider.GetTestFramework();
            var repo = provider.GetRequiredService<IWebContentRepository>();
            var command = provider.GetRequiredService<Mock<IContentDbCommand>>();
            var name = new Faker().Random.AlphaNumeric(length);
            command.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>()));

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await repo.Insert(new CreateContentRequest { Name = name });
            });
        }

        [Fact]
        public async Task RepoCanNotSetActiveRevisionWithEmptyName()
        {
            var provider = TestContextProvider.GetTestFramework();
            var repo = provider.GetRequiredService<IWebContentRepository>();
            var command = provider.GetRequiredService<Mock<IContentDbCommand>>();
            var dto = faker.Generate();
            dto.Id = string.Empty;

            command.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>()));

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await repo.SetActiveRevision(dto);
            });
        }

        [Fact]
        public async Task RepoCanNotSetActiveRevisionWithoutVersionId()
        {
            var provider = TestContextProvider.GetTestFramework();
            var repo = provider.GetRequiredService<IWebContentRepository>();
            var command = provider.GetRequiredService<Mock<IContentDbCommand>>();
            var dto = faker.Generate();
            dto.VersionId = null;

            command.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>()));

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await repo.SetActiveRevision(dto);
            });
        }

        [Fact]
        public async Task RepoCanSetActiveRevision()
        {
            var provider = TestContextProvider.GetTestFramework();
            var repo = provider.GetRequiredService<IWebContentRepository>();
            var command = provider.GetRequiredService<Mock<IContentDbCommand>>();
            var dto = faker.Generate();

            command.Setup(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>()));

            await repo.SetActiveRevision(dto);

            command.Verify(m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters?>()));
        }
    }
}