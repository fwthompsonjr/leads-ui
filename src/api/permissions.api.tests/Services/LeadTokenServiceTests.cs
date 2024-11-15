using legallead.permissions.api.Entities;
using legallead.permissions.api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace permissions.api.tests.Services
{
    public class LeadTokenServiceTests
    {
        [Fact]
        public void ServiceCanGenerateToken()
        {
            var reason = faker.Lorem.Sentence();
            var token = LeadTokenService.GenerateToken(reason, GetModel());
            Assert.NotNull(token);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ServiceCanGetModel(int conditionId)
        {
            var reason = faker.Lorem.Sentence();
            var token = LeadTokenService.GenerateToken(reason, GetModel());
            var encodedBytes = Encoding.UTF8.GetBytes(faker.Hacker.Phrase());
            var encodedString = Encoding.UTF8.GetString(encodedBytes);
            var convertedString = Convert.ToBase64String(encodedBytes);
            if (conditionId == 1) token = faker.Hacker.Phrase();
            if (conditionId == 2) token = Encoding.UTF8.GetString(
                Encoding.UTF8.GetBytes(faker.Hacker.Phrase()));
            if (conditionId == 3) token = encodedString;
            if (conditionId == 4) token = convertedString;
            var model = LeadTokenService.GetModel(token, out var expirationDt);
            if (conditionId == 0)
            {
                Assert.NotNull(expirationDt);
                Assert.NotNull(model);
            }
            else
            {
                Assert.Null(expirationDt);
                Assert.Null(model);
            }
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ServiceCanGetValidationModel(int conditionId)
        {
            var expected = conditionId == 0;
            var reason = faker.Lorem.Sentence();
            var token = LeadTokenService.GenerateToken(reason, GetModel());
            var model = LeadTokenService.GetValidationModel(token, expected ? reason : string.Empty);
            Assert.Equal(expected, model.Validated);
        }

        private static LeadUserModel GetModel()
        {
            var bo = LeadUserBoGenerator.GetBo(1, 1);
            return securityService.GetModel(bo);
        }

        private static readonly LeadSecurityService securityService = new ();
        private static readonly Faker faker = new ();
    }
}