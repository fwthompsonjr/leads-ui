using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class ComponentTests
    {
        private readonly Faker<Component> faker =
            new Faker<Component>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Name, y => y.Company.CompanyName());

        [Fact]
        public void ComponentCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new Component();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ComponentIsBaseDto()
        {
            var sut = new Component();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void ComponentHasTableNameDefined()
        {
            var expected = "applications";
            var sut = new Component();
            Assert.Equal(expected, sut.TableName);
        }

        [Fact]
        public void ComponentHasFieldListDefined()
        {
            var expected = new[] { "Id", "Name" };
            var sut = new Component();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Equal(expected.Length, fields.Count);
        }


        [Theory]
        [InlineData("Id")]
        [InlineData("Name")]
        public void ComponentHasExpectedFieldDefined(string name)
        {
            var sut = new Component();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Fact]
        public void ComponentCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void ComponentCanUpdateName()
        {
            var items = faker.Generate(2);
            items[0].Name = items[1].Name;
            Assert.Equal(items[1].Name, items[0].Name);
        }
        [Theory]
        [InlineData(0, "abcdefg")]
        [InlineData(1, "abcdefg")]
        public void ComponentCanReadWriteByIndex(int position, object expected)
        {
            var sut = new Component();
            sut[position] = expected;
            var actual = sut[position];
            Assert.Equal(expected, actual);
        }
    }
}