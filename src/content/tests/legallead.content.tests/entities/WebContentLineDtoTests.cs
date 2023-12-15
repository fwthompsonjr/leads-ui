using Bogus;
using legallead.content.entities;

namespace legallead.content.tests.entities
{
    public class WebContentLineDtoTests
    {
        private readonly Faker<WebContentLineDto> faker =
            new Faker<WebContentLineDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ContentId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(10, 20000))
            .RuleFor(x => x.InternalId, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.Content, y => y.Hacker.Phrase());

        [Fact]
        public void WebContentLineDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new WebContentLineDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void WebContentLineDtoIsBaseDto()
        {
            var sut = new WebContentLineDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<CommonBaseDto>(sut);
        }

        [Fact]
        public void WebContentLineDtoHasTableNameDefined()
        {
            var expected = "ContentLine";
            var sut = new WebContentLineDto();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Fact]
        public void WebContentLineDtoHasFieldListDefined()
        {
            var expected = new[] { "Id", "ContentId", "InternalId", "LineNbr", "Content" };
            var sut = new WebContentLineDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Equal(expected.Length, fields.Count);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("ContentId")]
        [InlineData("InternalId")]
        [InlineData("LineNbr")]
        [InlineData("Content")]
        public void WebContentLineDtoHasExpectedFieldDefined(string name)
        {
            var sut = new WebContentLineDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Fact]
        public void WebContentLineDtoCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void WebContentLineDtoCanUpdateContentId()
        {
            var items = faker.Generate(2);
            items[0].ContentId = items[1].ContentId;
            Assert.Equal(items[1].ContentId, items[0].ContentId);
        }

        [Fact]
        public void WebContentLineDtoCanUpdateInternalId()
        {
            var items = faker.Generate(2);
            items[0].InternalId = items[1].InternalId;
            Assert.Equal(items[1].InternalId, items[0].InternalId);
        }

        [Fact]
        public void WebContentLineDtoCanUpdateLineNbr()
        {
            var items = faker.Generate(2);
            items[0].LineNbr = items[1].LineNbr;
            Assert.Equal(items[1].LineNbr, items[0].LineNbr);
        }

        [Fact]
        public void WebContentLineDtoCanUpdateContent()
        {
            var items = faker.Generate(2);
            items[0].Content = items[1].Content;
            Assert.Equal(items[1].Content, items[0].Content);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void WebContentLineDtoCanReadByIndex(int position)
        {
            var sut = new WebContentLineDto();
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
        public void WebContentLineDtoCanWriteByIndex(int position)
        {
            var sut = faker.Generate(2);
            var fields = sut[0].FieldList;
            var fieldName = fields[position];
            var actual = sut[0][fieldName];
            sut[1][position] = actual;
            var expected = sut[1][fieldName];
            Assert.Equal(expected, actual);
        }
    }
}