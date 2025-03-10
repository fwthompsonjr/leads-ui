using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class LeadUserAccountDtoTests
    {

        private static readonly Faker<LeadUserAccountDto> faker =
            new Faker<LeadUserAccountDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Email, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.IsAdministrator, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void LeadUserAccountDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LeadUserAccountDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadUserAccountDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadUserAccountDtoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void LeadUserAccountDtoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Fact]
        public void LeadUserAccountDtoIsBaseDto()
        {
            var sut = new LeadUserAccountDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserName")]
        [InlineData("Email")]
        [InlineData("IsAdministrator")]
        [InlineData("CreateDate")]
        public void LeadUserAccountDtoHasExpectedFieldDefined(string name)
        {
            var sut = new LeadUserAccountDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserName")]
        [InlineData("Email")]
        [InlineData("IsAdministrator")]
        [InlineData("CreateDate")]
        public void LeadUserAccountDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new LeadUserAccountDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}