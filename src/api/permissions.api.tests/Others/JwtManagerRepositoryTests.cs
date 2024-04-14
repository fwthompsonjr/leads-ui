using legallead.jdbc.entities;
using legallead.permissions.api;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace permissions.api.tests.Others
{
    public class JwtManagerRepositoryTests
    {
        protected static readonly Faker<User> userfaker = new Faker<User>()
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());

        [Fact]
        public void RepoCanBeCreated()
        {
            var exception = Record.Exception(() => { _ = GetManager(); });
            Assert.Null(exception);
        }

        [Fact]
        public void RepoCanGenerateToken()
        {
            var exception = Record.Exception(() =>
            {
                var manager = GetManager();
                _ = manager.GenerateToken(userfaker.Generate());
            });
            Assert.Null(exception);
        }

        [Fact]
        public void RepoCanGetPrincipalFromExpiredToken()
        {
            var exception = Record.Exception(() =>
            {
                var manager = GetManager();
                var tokens = manager.GenerateToken(userfaker.Generate());
                var token = tokens?.AccessToken ?? string.Empty;
                _ = manager.GetPrincipalFromExpiredToken(token);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void RepoCanGenerateRefreshToken()
        {
            var exception = Record.Exception(() =>
            {
                var manager = GetManager();
                _ = manager.GenerateRefreshToken(userfaker.Generate());
            });
            Assert.Null(exception);
        }


        [Fact]
        public void RepoCanValidateToken()
        {
            var exception = Record.Exception(() =>
            {
                var manager = GetManager();
                var tokens = manager.GenerateToken(userfaker.Generate());
                var token = tokens?.AccessToken ?? string.Empty;
                var isvalid = manager.ValidateToken(token);
                Assert.True(isvalid);
            });
            Assert.Null(exception);
        }

        private static JwtManagerRepository GetManager()
        {
            return new JwtManagerRepository(GetConfiguration());
        }

        private static IConfiguration GetConfiguration()
        {
            const string environmentName = "Development";
            var config =
                new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();
            return config;
        }
    }
}
