using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class AppSettingBoTests
    {


        private static readonly Faker<AppSettingBo> faker =
            new Faker<AppSettingBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyValue, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Version, y => y.Random.Decimal(0, 2))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void AppSettingBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new AppSettingBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void AppSettingBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void AppSettingBoCanGetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.Id, dest.Id);
        }

        [Fact]
        public void AppSettingBoCanGetKeyName()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.KeyName, dest.KeyName);
        }

        [Fact]
        public void AppSettingBoCanGetKeyValue()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.KeyValue, dest.KeyValue);
        }

        [Fact]
        public void AppSettingBoCanGetVersion()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.Version, dest.Version);
        }

        [Fact]
        public void AppSettingBoCanGetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.CreateDate, dest.CreateDate);
        }
    }
}