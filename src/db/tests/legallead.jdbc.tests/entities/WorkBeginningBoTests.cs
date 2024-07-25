using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class WorkBeginningBoTests
    {


        private static readonly Faker<WorkIndexBo> idfaker =
            new Faker<WorkIndexBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"));

        private static readonly Faker<WorkBeginningBo> faker =
            new Faker<WorkBeginningBo>()
            .RuleFor(x => x.WorkIndexes, y =>
            {
                var n = y.Random.Int(1, 8);
                return idfaker.Generate(n);
            });

        [Fact]
        public void WorkBeginningBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                var tmp = new WorkBeginningBo();
                Assert.False(string.IsNullOrWhiteSpace(tmp.Source));
                Assert.Empty(tmp.WorkIndexes);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void WorkBeginningBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void WorkBeginningBoCanGetSource()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.Equal(src.Source, dest.Source);
        }

        [Fact]
        public void WorkBeginningBoCanSetSource()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            src.Source = dest.Source;
            Assert.Equal(src.Source, dest.Source);
        }

        [Fact]
        public void WorkBeginningBoCanSetWorkIndexes()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            src.WorkIndexes = dest.WorkIndexes;
            Assert.Equal(src.WorkIndexes, dest.WorkIndexes);
        }
    }
}