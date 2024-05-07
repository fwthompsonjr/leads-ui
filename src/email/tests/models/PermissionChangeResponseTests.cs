using legallead.email.models;

namespace legallead.email.tests.models
{
    public class PermissionChangeResponseTests
    {
        private static readonly Faker<PermissionLevelResponseBo> pfaker =
            new Faker<PermissionLevelResponseBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExternalId, y => y.Hacker.Phrase())
            .RuleFor(x => x.InvoiceUri, y => y.Hacker.Phrase())
            .RuleFor(x => x.LevelName, y => y.Hacker.Phrase())
            .RuleFor(x => x.SessionId, y => y.Hacker.Phrase())
            .RuleFor(x => x.IsPaymentSuccess, y => y.Random.Bool())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        private static readonly Faker<PermissionChangeResponse> faker
            = new Faker<PermissionChangeResponse>()
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Name, y => y.Music.Genre())
            .RuleFor(x => x.Request, y => y.Hacker.Phrase())
            .RuleFor(x => x.Dto, y => pfaker.Generate());


        [Fact]
        public void DtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void DtoHasExpectedFieldDefined()
        {
            var sut = new PermissionChangeResponse();
            var test = faker.Generate();
            Assert.NotEqual(sut.Email, test.Email);
            Assert.NotEqual(sut.Name, test.Name);
            Assert.NotEqual(sut.Request, test.Request);
            Assert.NotEqual(sut.Dto, test.Dto);
        }
    }
}
