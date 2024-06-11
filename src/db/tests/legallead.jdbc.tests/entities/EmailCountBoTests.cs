using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class EmailCountBoTests
    {


        private static readonly Faker<EmailCountBo> faker =
            MockEmailObjectProvider.CountBoFaker;


        [Fact]
        public void EmailCountBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new EmailCountBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void EmailCountBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void EmailCountBoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void EmailCountBoCanSetUserId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.UserId = src.UserId;
            Assert.Equal(src.UserId, dest.UserId);
        }

        [Fact]
        public void EmailCountBoCanSetItems()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Items = src.Items;
            Assert.Equal(src.Items, dest.Items);
        }
    }
}