using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class HoliDateBoTests
    {
        [Fact]
        public void ModelCanCreate()
        {
            var error = Record.Exception(() =>
            {
                _ = dfaker.Generate();
            });
            Assert.Null(error);
        }
        [Fact]
        public void ModelHasExpectedFields()
        {
            var error = Record.Exception(() =>
            {
                var sut = dfaker.Generate();
                Assert.False(string.IsNullOrEmpty(sut.Id));
                Assert.True(sut.HoliDate.HasValue);
                Assert.True(sut.CreateDate.HasValue);
            });
            Assert.Null(error);
        }

        private static readonly Faker<HoliDateBo> dfaker
            = new Faker<HoliDateBo>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.HoliDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());
    }
}