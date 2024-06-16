using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class EmailListBoTests
    {


        private static readonly Faker<EmailListBo> faker =
            MockEmailObjectProvider.ListBoFaker;


        [Fact]
        public void EmailListBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new EmailListBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void EmailListBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void EmailListBoCanSetUserId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.UserId = src.UserId;
            Assert.Equal(src.UserId, dest.UserId);
        }

        [Fact]
        public void EmailListBoCanSetFromAddress()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.FromAddress = src.FromAddress;
            Assert.Equal(src.FromAddress, dest.FromAddress);
        }

        [Fact]
        public void EmailListBoCanSetToAddress()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.ToAddress = src.ToAddress;
            Assert.Equal(src.ToAddress, dest.ToAddress);
        }

        [Fact]
        public void EmailListBoCanSetSubject()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Subject = src.Subject;
            Assert.Equal(src.Subject, dest.Subject);
        }

        [Fact]
        public void EmailListBoCanSetStatusId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.StatusId = src.StatusId;
            Assert.Equal(src.StatusId, dest.StatusId);
        }

        [Fact]
        public void EmailListBoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }
    }
}