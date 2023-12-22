using Bogus;
using legallead.logging.entities;

namespace legallead.logging.tests.entities
{
    public class LogContentDtoTests
    {
        private const int seed = 100;

        private readonly Faker<LogContentDto> faker =
            new Faker<LogContentDto>()
            .RuleFor(x => x.Id, y => y.IndexFaker + seed)
            .RuleFor(x => x.RequestId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StatusId, y => y.Random.Int(10, 2000))
            .RuleFor(x => x.LineNumber, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.NameSpace, y => y.Company.CompanyName())
            .RuleFor(x => x.ClassName, y => y.Company.CompanyName())
            .RuleFor(x => x.MethodName, y => y.Company.CompanyName())
            .RuleFor(x => x.Message, y => y.Company.CompanyName())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(300));

        [Fact]
        public void LogContentDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LogContentDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LogContentDtoIsBaseDto()
        {
            var sut = new LogContentDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<CommonBaseDto>(sut);
        }

        [Fact]
        public void LogContentDtoHasTableNameDefined()
        {
            var expected = "LOGCONTENT";
            var sut = new LogContentDto();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Fact]
        public void LogContentDtoHasFieldListDefined()
        {
            var expected = new[] {
                "Id",
                "RequestId",
                "StatusId",
                "LineNumber",
                "NameSpace",
                "ClassName",
                "MethodName",
                "Message",
                "CreateDate"
            };
            var sut = new LogContentDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Equal(expected.Length, fields.Count);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("RequestId")]
        [InlineData("StatusId")]
        [InlineData("LineNumber")]
        [InlineData("NameSpace")]
        [InlineData("ClassName")]
        [InlineData("MethodName")]
        [InlineData("Message")]
        [InlineData("CreateDate")]
        public void LogContentDtoHasExpectedFieldDefined(string name)
        {
            var sut = new LogContentDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Fact]
        public void LogContentDtoCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void LogContentDtoCanUpdateRequestId()
        {
            var items = faker.Generate(2);
            items[0].RequestId = items[1].RequestId;
            Assert.Equal(items[1].RequestId, items[0].RequestId);
        }

        [Fact]
        public void LogContentDtoCanUpdateStatusId()
        {
            var items = faker.Generate(2);
            items[0].StatusId = items[1].StatusId;
            Assert.Equal(items[1].StatusId, items[0].StatusId);
        }

        [Fact]
        public void LogContentDtoCanUpdateLineNumber()
        {
            var items = faker.Generate(2);
            items[0].LineNumber = items[1].LineNumber;
            Assert.Equal(items[1].LineNumber, items[0].LineNumber);
        }

        [Fact]
        public void LogContentDtoCanUpdateNameSpace()
        {
            var items = faker.Generate(2);
            items[0].NameSpace = items[1].NameSpace;
            Assert.Equal(items[1].NameSpace, items[0].NameSpace);
        }

        [Fact]
        public void LogContentDtoCanUpdateClassName()
        {
            var items = faker.Generate(2);
            items[0].ClassName = items[1].ClassName;
            Assert.Equal(items[1].ClassName, items[0].ClassName);
        }

        [Fact]
        public void LogContentDtoCanUpdateMethodName()
        {
            var items = faker.Generate(2);
            items[0].MethodName = items[1].MethodName;
            Assert.Equal(items[1].MethodName, items[0].MethodName);
        }

        [Fact]
        public void LogContentDtoCanUpdateMessage()
        {
            var items = faker.Generate(2);
            items[0].Message = items[1].Message;
            Assert.Equal(items[1].Message, items[0].Message);
        }

        [Fact]
        public void LogContentDtoCanUpdateCreateDate()
        {
            var items = faker.Generate(2);
            items[0].CreateDate = items[1].CreateDate;
            Assert.Equal(items[1].CreateDate, items[0].CreateDate);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void LogContentDtoCanReadByIndex(int position)
        {
            var sut = new LogContentDto();
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
        [InlineData(7)]
        [InlineData(8)]
        public void LogContentDtoCanWriteByIndex(int position)
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