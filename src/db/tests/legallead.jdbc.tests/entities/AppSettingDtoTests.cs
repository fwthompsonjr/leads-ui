using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class AppSettingDtoTests
    {

        private static readonly Faker<AppSettingDto> faker =
            new Faker<AppSettingDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyValue, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Version, y => y.Random.Decimal(0, 2))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());



        [Fact]
        public void AppSettingDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new AppSettingDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void AppSettingDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("KeyName")]
        [InlineData("KeyValue")]
        [InlineData("Version")]
        [InlineData("CreateDate")]
        public void AppSettingDtoHasExpectedFieldDefined(string name)
        {
            const string na = "notmapped";
            var sut = new AppSettingDto();
            var fields = sut.FieldList;
            sut[na] = na;
            _ = sut[na];
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("KeyName")]
        [InlineData("KeyValue")]
        [InlineData("Version")]
        [InlineData("CreateDate")]
        public void AppSettingDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new AppSettingDto();
            var flds = sut.FieldList;
            demo["id"] = null;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}