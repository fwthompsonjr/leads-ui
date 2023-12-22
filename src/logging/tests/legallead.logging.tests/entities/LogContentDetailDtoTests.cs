using Bogus;
using legallead.logging.entities;

namespace legallead.logging.tests.entities
{
    public class LogContentDetailDtoTests
    {
        private const int seed = 100;

        private readonly Faker<LogContentDetailDto> faker =
            new Faker<LogContentDetailDto>()
            .RuleFor(x => x.Id, y => y.IndexFaker + seed)
            .RuleFor(x => x.LogContentId, y => y.Random.Int(10, 20000))
            .RuleFor(x => x.LineId, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.Line, y => y.Company.CompanyName());

        [Fact]
        public void LogContentDetailDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LogContentDetailDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LogContentDetailDtoIsBaseDto()
        {
            var sut = new LogContentDetailDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<CommonBaseDto>(sut);
        }

        [Fact]
        public void LogContentDetailDtoHasTableNameDefined()
        {
            var expected = "LOGCONTENTDETAIL";
            var sut = new LogContentDetailDto();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Fact]
        public void LogContentDetailDtoHasFieldListDefined()
        {
            var expected = new[] {
                "Id",
                "LogContentId",
                "LineId",
                "Line"
            };
            var sut = new LogContentDetailDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Equal(expected.Length, fields.Count);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("LogContentId")]
        [InlineData("LineId")]
        [InlineData("Line")]
        public void LogContentDetailDtoHasExpectedFieldDefined(string name)
        {
            var sut = new LogContentDetailDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Fact]
        public void LogContentDetailDtoCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void LogContentDetailDtoCanUpdateLogContentId()
        {
            var items = faker.Generate(2);
            items[0].LogContentId = items[1].LogContentId;
            Assert.Equal(items[1].LogContentId, items[0].LogContentId);
        }

        [Fact]
        public void LogContentDetailDtoCanUpdateLineId()
        {
            var items = faker.Generate(2);
            items[0].LineId = items[1].LineId;
            Assert.Equal(items[1].LineId, items[0].LineId);
        }

        [Fact]
        public void LogContentDetailDtoCanUpdateLine()
        {
            var items = faker.Generate(2);
            items[0].Line = items[1].Line;
            Assert.Equal(items[1].Line, items[0].Line);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void LogContentDetailDtoCanReadByIndex(int position)
        {
            var sut = new LogContentDetailDto();
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
        public void LogContentDetailDtoCanWriteByIndex(int position)
        {
            var sut = faker.Generate(2);
            var fields = sut[0].FieldList;
            var fieldName = fields[position];
            var actual = sut[0][fieldName];
            sut[1][fieldName] = actual;
            var expected = sut[1][fieldName];
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LogContentDetailDtoCanGetLine()
        {
            var sut = new LogContentDetailDto();
            var expected = sut.Line;
            var actual = sut["Line"];
            Assert.Equal(expected, actual);
        }
    }
}