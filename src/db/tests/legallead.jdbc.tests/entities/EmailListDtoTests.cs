using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class EmailListDtoTests
    {

        private static readonly Faker<EmailListDto> faker =
            MockEmailObjectProvider.ListDtoFaker;


        [Fact]
        public void EmailListDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new EmailListDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void EmailListDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void EmailListDtoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void EmailListDtoCanSetUserId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.UserId = src.UserId;
            Assert.Equal(src.UserId, dest.UserId);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("FromAddress")]
        [InlineData("ToAddress")]
        [InlineData("Subject")]
        [InlineData("StatusId")]
        [InlineData("CreateDate")]
        public void EmailListDtoHasExpectedFieldDefined(string name)
        {
            var sut = new EmailListDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("FromAddress")]
        [InlineData("ToAddress")]
        [InlineData("Subject")]
        [InlineData("StatusId")]
        [InlineData("CreateDate")]
        public void EmailListDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new EmailListDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}