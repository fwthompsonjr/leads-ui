using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class PermissionGroupTests
    {
        private readonly Faker<PermissionGroup> faker =
            new Faker<PermissionGroup>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.GroupId, y => y.Random.Int(5, 1000))
            .RuleFor(x => x.OrderId, y => y.Random.Int(5, 1000))
            .RuleFor(x => x.PerRequest, y => y.Random.Int(5, 100000))
            .RuleFor(x => x.PerMonth, y => y.Random.Int(5, 100000))
            .RuleFor(x => x.PerYear, y => y.Random.Int(5, 100000))
            .RuleFor(x => x.IsActive, y => y.Random.Bool())
            .RuleFor(x => x.IsVisible, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void PermissionGroupCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new PermissionGroup();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PermissionGroupIsBaseDto()
        {
            var sut = new PermissionGroup();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void PermissionGroupHasTableNameDefined()
        {
            var expected = "permissiongroup";
            var sut = new PermissionGroup();
            Assert.Equal(expected, sut.TableName);
        }

        [Fact]
        public void PermissionGroupHasFieldListDefined()
        {
            var expected = new[] {
                "Id",
                "Name",
                "GroupId",
                "OrderId",
                "PerRequest",
                "PerMonth",
                "PerYear",
                "IsActive",
                "IsVisible",
                "CreateDate"
            };
            var sut = new PermissionGroup();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Equal(expected.Length, fields.Count);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("Name")]
        [InlineData("GroupId")]
        [InlineData("OrderId")]
        [InlineData("PerRequest")]
        [InlineData("PerMonth")]
        [InlineData("PerYear")]
        [InlineData("IsActive")]
        [InlineData("IsVisible")]
        [InlineData("CreateDate")]
        public void PermissionGroupHasExpectedFieldDefined(string name)
        {
            var sut = new PermissionGroup();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Fact]
        public void PermissionGroupCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void PermissionGroupCanUpdateName()
        {
            var items = faker.Generate(2);
            items[0].Name = items[1].Name;
            Assert.Equal(items[1].Name, items[0].Name);
        }

        [Fact]
        public void PermissionGroupCanUpdateGroupId()
        {
            var items = faker.Generate(2);
            items[0].GroupId = items[1].GroupId;
            Assert.Equal(items[1].GroupId, items[0].GroupId);
        }

        [Fact]
        public void PermissionGroupCanUpdateOrderId()
        {
            var items = faker.Generate(2);
            items[0].OrderId = items[1].OrderId;
            Assert.Equal(items[1].OrderId, items[0].OrderId);
        }

        [Fact]
        public void PermissionGroupCanUpdatePerRequest()
        {
            var items = faker.Generate(2);
            items[0].PerRequest = items[1].PerRequest;
            Assert.Equal(items[1].PerRequest, items[0].PerRequest);
        }

        [Fact]
        public void PermissionGroupCanUpdatePerMonth()
        {
            var items = faker.Generate(2);
            items[0].PerMonth = items[1].PerMonth;
            Assert.Equal(items[1].PerMonth, items[0].PerMonth);
        }

        [Fact]
        public void PermissionGroupCanUpdatePerYear()
        {
            var items = faker.Generate(2);
            items[0].PerYear = items[1].PerYear;
            Assert.Equal(items[1].PerYear, items[0].PerYear);
        }

        [Fact]
        public void PermissionGroupCanUpdateIsActive()
        {
            var items = faker.Generate(2);
            items[0].IsActive = items[1].IsActive;
            Assert.Equal(items[1].IsActive, items[0].IsActive);
        }

        [Fact]
        public void PermissionGroupCanUpdateIsVisible()
        {
            var items = faker.Generate(2);
            items[0].IsVisible = items[1].IsVisible;
            Assert.Equal(items[1].IsVisible, items[0].IsVisible);
        }

        [Theory]
        [InlineData("Id", "123-456-789")]
        [InlineData("Name", "RandomTask")]
        [InlineData("GroupId", 214)]
        [InlineData("OrderId", 972)]
        [InlineData("PerRequest", 25)]
        [InlineData("PerMonth", 1525)]
        [InlineData("PerYear", 25000)]
        [InlineData("IsActive", false)]
        [InlineData("IsVisible", false)]
        public void PermissionGroupCanReadWriteByIndex(string fieldName, object expected)
        {
            var sut = new PermissionGroup();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = expected;
            var actual = sut[position];
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PermissionGroupCanReadWriteCreateDate()
        {
            string fieldName = "CreateDate";
            var expected = faker.Generate().CreateDate;
            var sut = new PermissionGroup();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = expected;
            var actual = sut[position];
            Assert.Equal(expected, actual);
        }
    }
}