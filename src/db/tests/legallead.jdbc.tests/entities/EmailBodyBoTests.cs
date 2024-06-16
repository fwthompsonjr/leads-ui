using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class EmailBodyBoTests
    {


        private static readonly Faker<EmailBodyBo> faker =
            MockEmailObjectProvider.BodyBoFaker;


        [Fact]
        public void EmailBodyBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new EmailBodyBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void EmailBodyBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void EmailBodyBoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void EmailBodyBoCanSetBody()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Body = src.Body;
            Assert.Equal(src.Body, dest.Body);
        }
    }
}