using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class EmailCountDtoTests
    {

        private static readonly Faker<EmailCountDto> faker =
            MockEmailObjectProvider.CountDtoFaker;


        [Fact]
        public void EmailCountDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new EmailCountDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void EmailCountDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void EmailCountDtoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void EmailCountDtoCanSetItems()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Items = src.Items;
            Assert.Equal(src.Items, dest.Items);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("Items")]
        public void EmailCountDtoHasExpectedFieldDefined(string name)
        {
            var sut = new EmailCountDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("Items")]
        public void EmailCountDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new EmailCountDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}