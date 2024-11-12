using Bogus;
using legallead.jdbc.entities;
using System.Text;

namespace legallead.jdbc.tests.entities
{
    public class LeadUserDtoTests
    {

        private static readonly Faker<LeadUserDto> faker =
            new Faker<LeadUserDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Phrase, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Vector, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Token, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void LeadUserDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LeadUserDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadUserDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserName")]
        [InlineData("Phrase")]
        [InlineData("Vector")]
        [InlineData("Token")]
        [InlineData("CreateDate")]
        public void LeadUserDtoHasExpectedFieldDefined(string name)
        {
            const string na = "notmapped";
            var sut = new LeadUserDto();
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
        [InlineData("Phrase")]
        [InlineData("Vector")]
        [InlineData("Token")]
        [InlineData("CreateDate")]
        public void LeadUserDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new LeadUserDto();
            var flds = sut.FieldList;
            demo["id"] = null;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}