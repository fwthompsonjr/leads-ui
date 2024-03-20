using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class SubscriptionDetailDtoTests
    {

        private static readonly Faker<SubscriptionDetailDto> faker =
            new Faker<SubscriptionDetailDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CustomerId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Email, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SubscriptionType, y => y.Hacker.Phrase())
            .RuleFor(x => x.SubscriptionDetail, y => y.Hacker.Phrase())
            .RuleFor(x => x.PermissionLevel, y => y.Hacker.Phrase())
            .RuleFor(x => x.CountySubscriptions, y => y.Hacker.Phrase())
            .RuleFor(x => x.StateSubscriptions, y => y.Hacker.Phrase())
            .RuleFor(x => x.IsSubscriptionVerified, y => y.Random.Bool())
            .RuleFor(x => x.VerificationDate, y => y.Date.Recent())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void SubscriptionDetailDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SubscriptionDetailDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SubscriptionDetailDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("CustomerId")]
        [InlineData("Email")]
        [InlineData("ExternalId")]
        [InlineData("SubscriptionType")]
        [InlineData("SubscriptionDetail")]
        [InlineData("PermissionLevel")]
        [InlineData("CountySubscriptions")]
        [InlineData("StateSubscriptions")]
        [InlineData("IsSubscriptionVerified")]
        [InlineData("VerificationDate")]
        [InlineData("CompletionDate")]
        [InlineData("CreateDate")]
        public void SubscriptionDetailDtoHasExpectedFieldDefined(string name)
        {
            var sut = new SubscriptionDetailDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("CustomerId")]
        [InlineData("Email")]
        [InlineData("ExternalId")]
        [InlineData("SubscriptionType")]
        [InlineData("SubscriptionDetail")]
        [InlineData("PermissionLevel")]
        [InlineData("CountySubscriptions")]
        [InlineData("StateSubscriptions")]
        [InlineData("IsSubscriptionVerified")]
        [InlineData("VerificationDate")]
        [InlineData("CompletionDate")]
        [InlineData("CreateDate")]
        public void SubscriptionDetailDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new SubscriptionDetailDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}