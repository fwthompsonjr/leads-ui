using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class LeadUserProfileDtoTests
    {

        private static readonly Faker<LeadUserProfileDto> faker =
            new Faker<LeadUserProfileDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ProfileMapId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyValue, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyName, y => y.Random.Guid().ToString("D"));


        [Fact]
        public void LeadUserProfileDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LeadUserProfileDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadUserProfileDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadUserProfileDtoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void LeadUserProfileDtoIsBaseDto()
        {
            var sut = new LeadUserProfileDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("ProfileMapId")]
        [InlineData("KeyValue")]
        [InlineData("KeyName")]
        public void LeadUserProfileDtoHasExpectedFieldDefined(string name)
        {
            var sut = new LeadUserProfileDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("ProfileMapId")]
        [InlineData("KeyValue")]
        [InlineData("KeyName")]
        public void LeadUserProfileDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new LeadUserProfileDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}