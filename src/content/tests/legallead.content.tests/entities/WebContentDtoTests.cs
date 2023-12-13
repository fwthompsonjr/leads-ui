using Bogus;
using legallead.content.entities;

namespace legallead.content.tests.entities
{
    public class WebContentDtoTests
    {
        private readonly Faker<WebContentDto> faker =
            new Faker<WebContentDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UniqueId, y => y.Random.Int(10, 20000))
            .RuleFor(x => x.VersionId, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.IsActive, y => y.Random.Bool())
            .RuleFor(x => x.IsChild, y => y.Random.Bool())
            .RuleFor(x => x.ContentName, y => y.Company.CompanyName())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(300));

        [Fact]
        public void WebContentDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new WebContentDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void WebContentDtoIsBaseDto()
        {
            var sut = new WebContentDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<CommonBaseDto>(sut);
        }

        [Fact]
        public void WebContentDtoHasTableNameDefined()
        {
            var expected = "Content";
            var sut = new WebContentDto();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Fact]
        public void WebContentDtoHasFieldListDefined()
        {
            var expected = new[] { "Id", "UniqueId", "VersionId", "ContentName", "IsActive", "IsChild", "CreateDate" };
            var sut = new WebContentDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Equal(expected.Length, fields.Count);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UniqueId")]
        [InlineData("VersionId")]
        [InlineData("ContentName")]
        [InlineData("IsActive")]
        [InlineData("IsChild")]
        [InlineData("CreateDate")]
        public void WebContentDtoHasExpectedFieldDefined(string name)
        {
            var sut = new WebContentDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Fact]
        public void WebContentDtoCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void WebContentDtoCanUpdateUniqueId()
        {
            var items = faker.Generate(2);
            items[0].UniqueId = items[1].UniqueId;
            Assert.Equal(items[1].UniqueId, items[0].UniqueId);
        }

        [Fact]
        public void WebContentDtoCanUpdateVersionId()
        {
            var items = faker.Generate(2);
            items[0].VersionId = items[1].VersionId;
            Assert.Equal(items[1].VersionId, items[0].VersionId);
        }

        [Fact]
        public void WebContentDtoCanUpdateContentName()
        {
            var items = faker.Generate(2);
            items[0].ContentName = items[1].ContentName;
            Assert.Equal(items[1].ContentName, items[0].ContentName);
        }

        [Fact]
        public void WebContentDtoCanUpdateIsActive()
        {
            var items = faker.Generate(2);
            items[0].IsActive = items[1].IsActive;
            Assert.Equal(items[1].IsActive, items[0].IsActive);
        }

        [Fact]
        public void WebContentDtoCanUpdateIsChild()
        {
            var items = faker.Generate(2);
            items[0].IsChild = items[1].IsChild;
            Assert.Equal(items[1].IsChild, items[0].IsChild);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void WebContentDtoCanReadByIndex(int position)
        {
            var sut = new WebContentDto();
            var fields = sut.FieldList;
            var fieldName = fields[position];
            var actual = sut[position];
            var expected = sut[fieldName];
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void WebContentDtoCanWriteByIndex(int position)
        {
            var sut = faker.Generate(2);
            var fields = sut[0].FieldList;
            var fieldName = fields[position];
            var actual = sut[0][fieldName];
            sut[1][fieldName] = actual;
            var expected = sut[1][fieldName];
            Assert.Equal(expected, actual);
        }
    }
}