using Bogus;
using Dapper;
using legallead.email.entities;
using legallead.email.interfaces;
using legallead.email.models;
using legallead.email.services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Data;

namespace legallead.email.tests.services
{
    public class UserSettingInfrastructureTests
    {
        [Fact]
        public void SutCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                var provider = Provider;
                var service = provider.GetRequiredService<IUserSettingInfrastructure>();
                Assert.NotNull(service);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(10, false)]
        [InlineData(10, true, false)]
        [InlineData(10, false, false)]
        public async Task SutCanGetSettings(
            int count,
            bool hasEmail = true,
            bool hasIndex = true)
        {
            var id = hasIndex ? Guid.NewGuid().ToString() : null;
            var email = hasEmail ? "abc@test.com" : null;
            var provider = Provider;
            var conn = new Mock<IDbConnection>();
            var connect = provider.GetRequiredService<Mock<IDataConnectionService>>();
            var db = provider.GetRequiredService<Mock<IDataCommandService>>();
            var data = faker.Generate(count);
            var query = new UserSettingQuery { Email = email, Id = id };
            var isValid = query.IsValid;
            var service = provider.GetRequiredService<IUserSettingInfrastructure>();

            connect.Setup(m => m.CreateConnection()).Returns(conn.Object);
            if (isValid)
            {
                db.Setup(m => m.QueryAsync<UserEmailSettingDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(data).Verifiable(Times.Once());
            }
            else
            {
                db.Setup(m => m.QueryAsync<UserEmailSettingDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(data).Verifiable(Times.Never());
            }
            _ = await service.GetSettings(query);

            if (isValid) connect.Verify(m => m.CreateConnection());

        }
        private static IServiceProvider Provider
        {
            get
            {
                var services = new ServiceCollection();
                var commandMq = new Mock<IDataCommandService>();
                var connectMq = new Mock<IDataConnectionService>();
                services.AddSingleton(x => commandMq);
                services.AddSingleton(x => commandMq.Object);
                services.AddSingleton(x => connectMq);
                services.AddSingleton(x => connectMq.Object);
                services.AddSingleton<IUserSettingInfrastructure, UserSettingInfrastructure>();
                return services.BuildServiceProvider();
            }
        }
        private static readonly List<string> commonKeys =
        [
            "Email 1",
            "Email 2",
            "Email 3",
            "First Name",
            "Last Name",
        ];

        private static readonly Faker<UserEmailSettingDto> faker =
            new Faker<UserEmailSettingDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.KeyName, y => y.PickRandom(commonKeys))
            .FinishWith((a, b) =>
            {
                b.KeyValue = b.KeyName switch
                {
                    "First Name" => a.Person.FirstName,
                    "Last Name" => a.Person.LastName,
                    _ => a.Person.Email
                };
            });
    }
}
