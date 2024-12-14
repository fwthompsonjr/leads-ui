using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class DbCountyAppendLimitBoTests
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
            });
            Assert.Null(error);
        }

        private static readonly Faker<DbCountyAppendLimitBo> dfaker
            = new Faker<DbCountyAppendLimitBo>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25));
    }
}