using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class PaymentSessionDtoTests
    {

        private static readonly Faker<PaymentSessionDto> faker =
            new Faker<PaymentSessionDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CustomerId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.InvoiceId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SessionType, y => y.Hacker.Phrase())
            .RuleFor(x => x.SessionId, y => y.Hacker.Phrase())
            .RuleFor(x => x.IntentId, y => y.Hacker.Phrase())
            .RuleFor(x => x.ClientId, y => y.Hacker.Phrase())
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.JsText, y => y.Hacker.Phrase())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void PaymentSessionDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new PaymentSessionDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PaymentSessionDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PaymentSessionDtoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("CustomerId")]
        [InlineData("InvoiceId")]
        [InlineData("SessionType")]
        [InlineData("SessionId")]
        [InlineData("IntentId")]
        [InlineData("ClientId")]
        [InlineData("ExternalId")]
        [InlineData("JsText")]
        [InlineData("CreateDate")]
        public void PaymentSessionDtoHasExpectedFieldDefined(string name)
        {
            var sut = new PaymentSessionDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("CustomerId")]
        [InlineData("InvoiceId")]
        [InlineData("SessionType")]
        [InlineData("SessionId")]
        [InlineData("IntentId")]
        [InlineData("ClientId")]
        [InlineData("ExternalId")]
        [InlineData("JsText")]
        [InlineData("CreateDate")]
        public void PaymentSessionDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new PaymentSessionDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}