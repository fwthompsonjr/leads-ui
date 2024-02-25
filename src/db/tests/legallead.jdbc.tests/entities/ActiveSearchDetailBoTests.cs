using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class ActiveSearchDetailBoTests
    {

        private static readonly Faker<ActiveSearchDetailBo> faker =
            new Faker<ActiveSearchDetailBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StateName, y => y.Hacker.Phrase())
            .RuleFor(x => x.StartDate, y => y.Hacker.Phrase())
            .RuleFor(x => x.EndDate, y => y.Hacker.Phrase())
            .RuleFor(x => x.SearchProgress, y => y.Hacker.Phrase())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void ActiveSearchDetailBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ActiveSearchDetailBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ActiveSearchDetailBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ActiveSearchDetailBoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void ActiveSearchDetailBoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Fact]
        public void ActiveSearchDetailBoCanSetCountyName()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CountyName = src.CountyName;
            Assert.Equal(src.CountyName, dest.CountyName);
        }
        [Fact]
        public void ActiveSearchDetailBoCanSetStateName()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.StateName = src.StateName;
            Assert.Equal(src.StateName, dest.StateName);
        }

        [Fact]
        public void ActiveSearchDetailBoCanSetStartDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.StartDate = src.StartDate;
            Assert.Equal(src.StartDate, dest.StartDate);
        }

        [Fact]
        public void ActiveSearchDetailBoCanSetEndDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.EndDate = src.EndDate;
            Assert.Equal(src.EndDate, dest.EndDate);
        }

        [Fact]
        public void ActiveSearchDetailBoCanSetSearchProgress()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.SearchProgress = src.SearchProgress;
            Assert.Equal(src.SearchProgress, dest.SearchProgress);
        }
    }
}