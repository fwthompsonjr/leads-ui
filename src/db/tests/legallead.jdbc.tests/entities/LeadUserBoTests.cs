using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class LeadUserBoTests
    {



        private static readonly Faker<LeadUserBo> faker =
            new Faker<LeadUserBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserData, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.IndexData, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyData, y => y.Random.Guid().ToString("D"));

        [Fact]
        public void LeadUserBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LeadUserBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadUserBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadUserBoCanGetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.Id, dest.Id);
        }

        [Fact]
        public void LeadUserBoCanGetUserName()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.UserName, dest.UserName);
        }


        [Fact]
        public void LeadUserBoCanGetUserData()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.UserData, dest.UserData);
        }

        [Fact]
        public void LeadUserBoCanGetIndexData()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.IndexData, dest.IndexData);
        }

        [Fact]
        public void LeadUserBoCanGetIndex()
        {
            Assert.Equal(0, LeadUserBo.UserIndex);
            Assert.Equal(1, LeadUserBo.CountyIndex);
            Assert.Equal(2, LeadUserBo.DataIndex);
        }

        [Fact]
        public void LeadUserBoCanGetKeys()
        {
            var sut = new LeadUserBo();
            Assert.NotEmpty(sut.Keys);
            Assert.Equal(3, sut.Keys.Count);
        }
    }
}