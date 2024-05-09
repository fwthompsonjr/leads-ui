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


        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [InlineData(false, false)]
        public async Task SutCanGetUserByEmail(bool hasResponse, bool hasEmail = true)
        {
            var data = hasResponse ? ConvertFrom(accountfaker.Generate()) : null;
            var provider = Provider;
            var conn = new Mock<IDbConnection>();
            var connect = provider.GetRequiredService<Mock<IDataConnectionService>>();
            var db = provider.GetRequiredService<Mock<IDataCommandService>>();
            var query = accountfaker.Generate();
            if (!hasEmail) query.Email = string.Empty;
            var service = provider.GetRequiredService<IUserSettingInfrastructure>();

            connect.Setup(m => m.CreateConnection()).Returns(conn.Object);

            db.Setup(m => m.QuerySingleOrDefaultAsync<GetUserAccountByEmailDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).ReturnsAsync(data);
            _ = await service.GetUserByEmail(query.Email);

            if (hasEmail) connect.Verify(m => m.CreateConnection(), Times.Once);
            else connect.Verify(m => m.CreateConnection(), Times.Never);
        }


        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [InlineData(false, false)]
        public async Task SutCanGetUserByUserName(bool hasResponse, bool hasUserName = true)
        {
            var data = hasResponse ? userNameFaker.Generate() : null;
            var provider = Provider;
            var conn = new Mock<IDbConnection>();
            var connect = provider.GetRequiredService<Mock<IDataConnectionService>>();
            var db = provider.GetRequiredService<Mock<IDataCommandService>>();
            var query = accountfaker.Generate();
            if (!hasUserName) query.UserName = string.Empty;
            var service = provider.GetRequiredService<IUserSettingInfrastructure>();

            connect.Setup(m => m.CreateConnection()).Returns(conn.Object);

            db.Setup(m => m.QuerySingleOrDefaultAsync<GetUserAccountByUserNameDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).ReturnsAsync(data);
            _ = await service.GetUserByUserName(query.UserName);

            if (hasUserName) connect.Verify(m => m.CreateConnection(), Times.Once);
            else connect.Verify(m => m.CreateConnection(), Times.Never);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [InlineData(false, false)]
        public async Task SutCanGetUserBySearchId(bool hasResponse, bool hasIndex = true)
        {
            var data = hasResponse ? queryIndexFaker.Generate() : null;
            var provider = Provider;
            var conn = new Mock<IDbConnection>();
            var connect = provider.GetRequiredService<Mock<IDataConnectionService>>();
            var db = provider.GetRequiredService<Mock<IDataCommandService>>();
            var query = accountfaker.Generate();
            if (!hasIndex) query.Id = string.Empty;
            var service = provider.GetRequiredService<IUserSettingInfrastructure>();

            connect.Setup(m => m.CreateConnection()).Returns(conn.Object);

            db.Setup(m => m.QuerySingleOrDefaultAsync<GetUserAccountBySearchIndexDto>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>())).ReturnsAsync(data);
            _ = await service.GetUserBySearchId(query.Id ?? string.Empty);

            if (hasIndex) connect.Verify(m => m.CreateConnection(), Times.Once);
            else connect.Verify(m => m.CreateConnection(), Times.Never);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SutCanLog(bool hasIndex)
        {
            var id = hasIndex ? Guid.NewGuid().ToString() : null;
            var provider = Provider;
            var conn = new Mock<IDbConnection>();
            var connect = provider.GetRequiredService<Mock<IDataConnectionService>>();
            var db = provider.GetRequiredService<Mock<IDataCommandService>>();
            var data = hasIndex ? new LogCorrespondenceDto { Id = id ?? string.Empty } : null;
            var query = logFaker.Generate();
            var service = provider.GetRequiredService<IUserSettingInfrastructure>();

            connect.Setup(m => m.CreateConnection()).Returns(conn.Object);
            db.Setup(m => m.QuerySingleOrDefaultAsync<LogCorrespondenceDto>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ReturnsAsync(data).Verifiable(Times.Once());
            _ = await service.Log(query.Id, query.JsonData ?? string.Empty);

            connect.Verify(m => m.CreateConnection());
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SutCanLogSuccess(bool hasError)
        {
            var provider = Provider;
            var conn = new Mock<IDbConnection>();
            var connect = provider.GetRequiredService<Mock<IDataConnectionService>>();
            var db = provider.GetRequiredService<Mock<IDataCommandService>>();
            var query = logFaker.Generate();
            var exception = new Faker().System.Exception();
            var service = provider.GetRequiredService<IUserSettingInfrastructure>();

            connect.Setup(m => m.CreateConnection()).Returns(conn.Object);
            if (!hasError)
            {
                db.Setup(m => m.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).Verifiable(Times.Once());
            }
            else
            {
                db.Setup(m => m.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(exception).Verifiable(Times.Once());
            }
            service.LogSuccess(query.Id);

            connect.Verify(m => m.CreateConnection());
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SutCanLogError(bool hasError)
        {
            var provider = Provider;
            var conn = new Mock<IDbConnection>();
            var connect = provider.GetRequiredService<Mock<IDataConnectionService>>();
            var db = provider.GetRequiredService<Mock<IDataCommandService>>();
            var query = logFaker.Generate();
            var exception = new Faker().System.Exception();
            var service = provider.GetRequiredService<IUserSettingInfrastructure>();

            connect.Setup(m => m.CreateConnection()).Returns(conn.Object);
            if (!hasError)
            {
                db.Setup(m => m.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).Verifiable(Times.Once());
            }
            else
            {
                db.Setup(m => m.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>())).ThrowsAsync(exception).Verifiable(Times.Once());
            }
            service.LogError(query.Id, query.JsonData ?? string.Empty);

            connect.Verify(m => m.CreateConnection());
        }


        private static GetUserAccountByEmailDto ConvertFrom(UserAccountByEmailBo source)
        {
            return new GetUserAccountByEmailDto
            {
                Id = source.Id ?? string.Empty,
                Email = source.Email,
                UserName = source.UserName,
            };
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
        private static readonly Faker<LogCorrespondenceDto> logFaker
            = new Faker<LogCorrespondenceDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.JsonData, y =>
            {
                var obj = new { Id = "abc", Number = y.Random.Int(1, 1000), Text = y.Lorem.Paragraph() };
                return JsonConvert.SerializeObject(obj);
            });

        private static readonly Faker<UserAccountByEmailBo> accountfaker =
            new Faker<UserAccountByEmailBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.UserName, y => y.Person.UserName);

        private static readonly Faker<GetUserAccountBySearchIndexDto> queryIndexFaker =
            new Faker<GetUserAccountBySearchIndexDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.UserName, y => y.Person.UserName);

        private static readonly Faker<GetUserAccountByUserNameDto> userNameFaker =
            new Faker<GetUserAccountByUserNameDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.UserName, y => y.Person.UserName);
    }
}
