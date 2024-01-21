using Bogus;
using legallead.jdbc.entities;
using System.Text;

namespace legallead.jdbc.tests.entities
{
    public class BackgroundComponentHealthDtoTests
    {

        private static readonly Faker<BackgroundComponentHealthDto> faker =
            new Faker<BackgroundComponentHealthDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ComponentId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.HealthId, y => y.Random.Int(5, 25055))
            .RuleFor(x => x.Health, y => y.Hacker.Phrase())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void BackgroundComponentHealthDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new BackgroundComponentHealthDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void BackgroundComponentHealthDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }


        [Fact]
        public void BackgroundComponentHealthDtoIsBaseDto()
        {
            var sut = new BackgroundComponentHealthDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void BackgroundComponentHealthDtoHasTableNameDefined()
        {
            var expected = "BGCOMPONENTHEATH";
            var sut = new BackgroundComponentHealthDto();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("ComponentId")]
        [InlineData("HealthId")]
        [InlineData("Health")]
        [InlineData("CreateDate")]
        public void BackgroundComponentHealthDtoHasExpectedFieldDefined(string name)
        {
            var sut = new BackgroundComponentHealthDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("ComponentId")]
        [InlineData("HealthId")]
        [InlineData("Health")]
        [InlineData("CreateDate")]
        public void BackgroundComponentHealthDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new BackgroundComponentHealthDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}