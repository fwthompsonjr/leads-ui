using legallead.jdbc.entities;
using legallead.permissions.api;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace permissions.api.tests.Others
{
    public class RefreshTokenValidatorTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ModelCanBeCreated(bool isEmpty)
        {
            var exception = Record.Exception(() => { _ = GetValidator(isEmpty); });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ModelCanBeVerified(int condition)
        {
            var exception = Record.Exception(() =>
            {
                var validator = GetValidator();
                UserRefreshToken? token = condition == 0 ? null : tokenFaker.Generate();
                if(condition == 1 && token != null) { token.CreateDate = null; }
                validator.Verify(token);
            });
            Assert.Null(exception);
        }

        private static readonly Faker<UserRefreshToken> tokenFaker =
            new Faker<UserRefreshToken>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.RefreshToken, y => y.Hacker.Phrase())
            .RuleFor(x => x.IsActive, y => true)
            .RuleFor(x => x.CreateDate, y => { var hh = y.Random.Int(10, 15); return DateTime.UtcNow.AddHours(-hh); });

        private static RefreshTokenValidator GetValidator(bool isZeroWindow = false)
        {
            return new RefreshTokenValidator(GetConfiguration(isZeroWindow));
        }

        private static IConfiguration GetConfiguration(bool isZeroWindow = false)
        {
            const string environmentName = "Development";
            var config =
                new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();
            if (isZeroWindow) { config["RefreshWindow"] = "0"; }
            return config;
        }
    }
}
/*
 * 
        

*/