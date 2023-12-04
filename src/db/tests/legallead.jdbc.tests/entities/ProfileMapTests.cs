using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class ProfileMapTests
    {
        private readonly Faker<ProfileMap> faker =
            new Faker<ProfileMap>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.OrderId, y => y.Random.Int(1, 10000))
            .RuleFor(x => x.KeyName, y => y.Company.CompanyName());

        [Fact]
        public void ProfileMapCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ProfileMap();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ProfileMapIsBaseDto()
        {
            var sut = new ProfileMap();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void ProfileMapHasTableNameDefined()
        {
            var expected = "profilemap";
            var sut = new ProfileMap();
            Assert.Equal(expected, sut.TableName);
        }

        [Fact]
        public void ProfileMapHasFieldListDefined()
        {
            var expected = new[] { "Id", "OrderId", "KeyName" };
            var sut = new ProfileMap();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Equal(expected.Length, fields.Count);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("OrderId")]
        [InlineData("KeyName")]
        public void ProfileMapHasExpectedFieldDefined(string name)
        {
            var sut = new ProfileMap();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Fact]
        public void ProfileMapCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void ProfileMapCanUpdateOrderId()
        {
            var items = faker.Generate(2);
            items[0].OrderId = items[1].OrderId;
            Assert.Equal(items[1].OrderId, items[0].OrderId);
        }

        [Fact]
        public void ProfileMapCanUpdateKeyName()
        {
            var items = faker.Generate(2);
            items[0].KeyName = items[1].KeyName;
            Assert.Equal(items[1].KeyName, items[0].KeyName);
        }

        [Theory]
        [InlineData("Id", "abcdefg")]
        [InlineData("OrderId", 200)]
        [InlineData("KeyName", "15226")]
        public void ProfileMapCanReadWriteByIndex(string fieldName, object expected)
        {
            var sut = new ProfileMap();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = expected;
            var actual = sut[position];
            Assert.Equal(expected, actual);
        }
    }
}