using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class DbInvoiceViewDtoTests
    {
        [Fact]
        public void ModelCanCreate()
        {
            var error = Record.Exception(() =>
            {
                _ = dfaker.Generate();
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserName")]
        [InlineData("Email")]
        [InlineData("LeadUserId")]
        [InlineData("RequestId")]
        [InlineData("InvoiceNbr")]
        [InlineData("InvoiceUri")]
        [InlineData("RecordCount")]
        [InlineData("LineNbr")]
        [InlineData("Description")]
        [InlineData("ItemCount")]
        [InlineData("ItemPrice")]
        [InlineData("ItemTotal")]
        [InlineData("InvoiceTotal")]
        [InlineData("CreateDate")]
        [InlineData("CompleteDate")]
        public void ModelHasExpectedFieldDefined(string name)
        {
            const string na = "notmapped";
            var sut = dfaker.Generate();
            var fields = sut.FieldList;
            sut[na] = na;
            _ = sut[na];
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }


        [Theory]
        [InlineData("Id")]
        [InlineData("UserName")]
        [InlineData("Email")]
        [InlineData("LeadUserId")]
        [InlineData("RequestId")]
        [InlineData("InvoiceNbr")]
        [InlineData("InvoiceUri")]
        [InlineData("RecordCount")]
        [InlineData("LineNbr")]
        [InlineData("Description")]
        [InlineData("ItemCount")]
        [InlineData("ItemPrice")]
        [InlineData("ItemTotal")]
        [InlineData("InvoiceTotal")]
        [InlineData("CreateDate")]
        [InlineData("CompleteDate")]
        public void ModelCanReadWriteByIndex(string fieldName)
        {
            var demo = dfaker.Generate();
            var sut = dfaker.Generate();
            demo["id"] = null;
            sut[fieldName] = demo[fieldName];
            var actual = sut[fieldName];
            Assert.Equal(demo[fieldName], actual);
        }

        private static readonly Faker<DbInvoiceViewDto> dfaker
            = new Faker<DbInvoiceViewDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.UserName, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.Email, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.LeadUserId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.RequestId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.InvoiceNbr, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.InvoiceUri, y => y.Random.AlphaNumeric(40))
            .RuleFor(x => x.RecordCount, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(1, 100))
            .RuleFor(x => x.Description, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.ItemCount, y => y.Random.Int(1, 100))
            .RuleFor(x => x.ItemPrice, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.ItemTotal, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.InvoiceTotal, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.CompleteDate, y => y.Date.Recent());
    }
}