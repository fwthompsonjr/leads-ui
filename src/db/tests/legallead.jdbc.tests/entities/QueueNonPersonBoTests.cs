using Bogus;
using legallead.jdbc.entities;
using System.Text;

namespace legallead.jdbc.tests.entities
{
    public class QueueNonPersonBoTests
    {
        private static readonly Faker<QueueNonPersonBo> faker =
            new Faker<QueueNonPersonBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExpectedRows, y => y.Random.Int(0, 750000))
            .RuleFor(x => x.SearchProgress, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StateCode, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.EndDate, y => y.Date.Recent())
            .RuleFor(x => x.StartDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.ExcelData, y =>
            {
                var text = y.Hacker.Phrase();
                return Encoding.UTF8.GetBytes(text);
            });


        [Fact]
        public void QueueNonPersonBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new QueueNonPersonBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void QueueNonPersonBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void QueueNonPersonBoCanGetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.Id, dest.Id);
        }

        [Fact]
        public void QueueNonPersonBoCanGetUserId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.UserId, dest.UserId);
        }

        [Fact]
        public void QueueNonPersonBoCanGetExpectedRows()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.ExpectedRows, dest.ExpectedRows);
        }

        [Fact]
        public void QueueNonPersonBoCanGetSearchProgress()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.SearchProgress, dest.SearchProgress);
        }

        [Fact]
        public void QueueNonPersonBoCanGetStateCode()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.StateCode, dest.StateCode);
        }

        [Fact]
        public void QueueNonPersonBoCanGetCountyName()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.CountyName, dest.CountyName);
        }

        [Fact]
        public void QueueNonPersonBoCanGetEndDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.EndDate, dest.EndDate);
        }

        [Fact]
        public void QueueNonPersonBoCanGetStartDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.StartDate, dest.StartDate);
        }

        [Fact]
        public void QueueNonPersonBoCanGetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.CreateDate, dest.CreateDate);
        }

        [Fact]
        public void QueueNonPersonBoCanGetExcelData()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.ExcelData, dest.ExcelData);
        }
    }
}