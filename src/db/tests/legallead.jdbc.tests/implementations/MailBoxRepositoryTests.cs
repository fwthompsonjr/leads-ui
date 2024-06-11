using Bogus;
using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;
using System.Data;
using System.Text;

namespace legallead.jdbc.tests.implementations
{
    public class MailBoxRepositoryTests
    {

        [Fact]
        public void RepoCanBeConstructed()
        {
            var provider = new RepoContainer();
            var repo = provider.Repository;
            Assert.NotNull(repo);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GetCountSequenceTests(int index)
        {
            var faker = new Faker();
            var uuid = faker.Random.AlphaNumeric(8);
            var error = faker.System.Exception();
            var completion = index switch
            {
                0 => null,
                _ => MockEmailObjectProvider.CountDtoFaker.Generate()
            };
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            if (index == 2)
            {
                mock.Setup(m => m.QuerySingleOrDefaultAsync<EmailCountDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()))
                    .ThrowsAsync(error);
            } 
            else
            {
                mock.Setup(m => m.QuerySingleOrDefaultAsync<EmailCountDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()))
                    .ReturnsAsync(completion);
            }
            _ = await service.GetCount(uuid);
            mock.Verify(m => m.QuerySingleOrDefaultAsync<EmailCountDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task GetBodySequenceTests(int index)
        {
            var faker = new Faker();
            var uuid = faker.Random.AlphaNumeric(8);
            var messageId = faker.Random.AlphaNumeric(8);
            var error = faker.System.Exception();
            var completion = index switch
            {
                0 => null,
                _ => MockEmailObjectProvider.BodyDtoFaker.Generate()
            };
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            if (completion != null && index > 2)
            {
                completion.Body = EncodeBase64(completion.Body);
            }
            if (index == 2)
            {
                mock.Setup(m => m.QuerySingleOrDefaultAsync<EmailBodyDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()))
                    .ThrowsAsync(error);
            }
            else
            {
                mock.Setup(m => m.QuerySingleOrDefaultAsync<EmailBodyDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()))
                    .ReturnsAsync(completion);
            }
            var response = await service.GetBody(messageId, uuid);
            mock.Verify(m => m.QuerySingleOrDefaultAsync<EmailBodyDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
            if (completion == null || response == null) return;
            var actual = response.Body;
            var original = completion.Body;
            if (index >2)
            {
                Assert.NotEqual(original, actual);
            } 
            else
            {
                Assert.Equal(original, actual);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task GetMailMessagesSequenceTests(int index)
        {
            var faker = new Faker();
            var uuid = faker.Random.AlphaNumeric(8);
            DateTime? lastUpdate = index == 5 ? null : faker.Date.Recent();
            var error = faker.System.Exception();
            var completion = MockEmailObjectProvider.ListDtoFaker.Generate(index);
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            if (index == 2)
            {
                mock.Setup(m => m.QueryAsync<EmailListDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()))
                    .ThrowsAsync(error);
            }
            else
            {
                mock.Setup(m => m.QueryAsync<EmailListDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()))
                    .ReturnsAsync(completion);
            }
            _ = await service.GetMailMessages(uuid, lastUpdate);
            mock.Verify(m => m.QueryAsync<EmailListDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }



        private static string? EncodeBase64(string? source)
        {
            if (string.IsNullOrWhiteSpace(source)) return source;
            var result = Encoding.UTF8.GetBytes(source);
            return Convert.ToBase64String(result);
        }

        private sealed class RepoContainer
        {
            private readonly IMailBoxRepository repo;
            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {

                command = new Mock<IDapperCommand>();
                var dataContext = new DataContext(command.Object);
                repo = new MailBoxRepository(dataContext);
            }

            public IMailBoxRepository Repository => repo;
            public Mock<IDapperCommand> CommandMock => command;
        }
    }
}