using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class WorkIndexBoTests
    {


        private static readonly Faker<WorkIndexBo> faker =
            new Faker<WorkIndexBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"));


        [Fact]
        public void WorkIndexBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new WorkIndexBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void WorkIndexBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void WorkIndexBoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }
    }
}