using Bogus;
using legallead.jdbc.entities;
using System.Text;

namespace legallead.jdbc.tests.entities
{
    public class QueueNonPersonDtoTests
    {

        private static readonly Faker<QueueNonPersonDto> faker =
            new Faker<QueueNonPersonDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExpectedRows, y => y.Random.Int(0, 750000))
            .RuleFor(x => x.SearchProgress, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StateCode, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.EndDate, y => y.Date.Recent())
            .RuleFor(x => x.StartDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.ExcelData, y =>
            {
                var text = y.Hacker.Phrase();
                return Encoding.UTF8.GetBytes(text);
            });



        [Fact]
        public void QueueNonPersonDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new QueueNonPersonDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void QueueNonPersonDtoCanBeGenerated()
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
        [InlineData("ExpectedRows")]
        [InlineData("SearchProgress")]
        [InlineData("StateCode")]
        [InlineData("CountyName")]
        [InlineData("EndDate")]
        [InlineData("StartDate")]
        [InlineData("CreateDate")]
        [InlineData("ExcelData")]
        public void QueueNonPersonDtoHasExpectedFieldDefined(string name)
        {
            const string na = "notmapped";
            var sut = new QueueNonPersonDto();
            var fields = sut.FieldList;
            sut[na] = na;
            _ = sut[na];
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }


        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("ExpectedRows")]
        [InlineData("SearchProgress")]
        [InlineData("StateCode")]
        [InlineData("CountyName")]
        [InlineData("EndDate")]
        [InlineData("StartDate")]
        [InlineData("CreateDate")]
        [InlineData("ExcelData")]
        public void QueueNonPersonDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new QueueNonPersonDto();
            var flds = sut.FieldList;
            demo["id"] = null;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}