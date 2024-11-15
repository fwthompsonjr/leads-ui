using legallead.permissions.api;
using legallead.permissions.api.Model;
using legallead.permissions.api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace permissions.api.tests.Models
{
    public class UserCountyCredentialModelTests
    {

        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserCountyCredentialModel();
            });
            Assert.Null(exception);
        }


        [Fact]
        public void ModelCanGenerate()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ModelValidationTests(int conditionId, string countyName = "dallas")
        {
            var error = Record.Exception(() =>
            {
                var expected = conditionId == 0;
                var sut = faker.Generate();
                sut.CountyName = countyName;
                if (conditionId == 1) sut.CountyName = string.Empty;
                if (conditionId == 2) sut.UserName = string.Empty;
                if (conditionId == 3) sut.Password = string.Empty;
                _ = sut.Validate(out var actual);
                Assert.Equal(expected, actual);
            });
            Assert.Null(error);
        }
        private static readonly Faker<UserCountyCredentialModel> faker =
            new Faker<UserCountyCredentialModel>()
            .RuleFor(x => x.UserName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Password, y => y.Random.AlphaNumeric(22))
            .RuleFor(x => x.CountyName, y => y.Address.County());
    }
}
