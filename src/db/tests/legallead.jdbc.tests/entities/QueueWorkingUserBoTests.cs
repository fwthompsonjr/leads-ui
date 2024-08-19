using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class QueueWorkingUserBoTests
    {


        private static readonly Faker<QueueWorkingUserBo> faker =
            new Faker<QueueWorkingUserBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.Email, y => y.Person.Email);

        [Fact]
        public void QueueWorkingUserBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new QueueWorkingUserBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void QueueWorkingUserBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void QueueWorkingUserBoCanGetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.Id, dest.Id);
        }

        [Fact]
        public void QueueWorkingUserBoCanGetUserName()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.UserName, dest.UserName);
        }

        [Fact]
        public void QueueWorkingUserBoCanGetEmail()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.Email, dest.Email);
        }
    }
}