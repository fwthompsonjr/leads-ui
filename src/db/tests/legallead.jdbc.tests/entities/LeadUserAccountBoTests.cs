using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class LeadUserAccountBoTests
    {

        private static readonly Faker<LeadUserAccountBo> faker =
            new Faker<LeadUserAccountBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Email, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.IsAdministrator, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void LeadUserAccountBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LeadUserAccountBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadUserAccountBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadUserAccountBoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void LeadUserAccountBoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Fact]
        public void LeadUserAccountBoCanSetUserName()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.UserName = src.UserName;
            Assert.Equal(src.UserName, dest.UserName);
        }

        [Fact]
        public void LeadUserAccountBoCanSetEmail()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Email = src.Email;
            Assert.Equal(src.Email, dest.Email);
        }
        [Fact]
        public void LeadUserAccountBoCanSetIsAdministrator()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.IsAdministrator = src.IsAdministrator;
            Assert.Equal(src.IsAdministrator, dest.IsAdministrator);
        }
    }
}