using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class EmailBodyDtoTests
    {

        private static readonly Faker<EmailBodyDto> faker =
            MockEmailObjectProvider.BodyDtoFaker;


        [Fact]
        public void EmailBodyDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new EmailBodyDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void EmailBodyDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void EmailBodyDtoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void EmailBodyDtoCanSetBody()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Body = src.Body;
            Assert.Equal(src.Body, dest.Body);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("Body")]
        public void EmailBodyDtoHasExpectedFieldDefined(string name)
        {
            var sut = new EmailBodyDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("Body")]
        public void EmailBodyDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new EmailBodyDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}