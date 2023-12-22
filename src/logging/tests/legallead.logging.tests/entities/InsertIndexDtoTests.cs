using Bogus;
using legallead.logging.entities;

namespace legallead.logging.tests.entities
{
    public class InsertIndexDtoTests
    {
        private const int seed = 100;

        private readonly Faker<InsertIndexDto> faker =
            new Faker<InsertIndexDto>()
            .RuleFor(x => x.Id, y => y.IndexFaker + seed);

        [Fact]
        public void InsertIndexDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new InsertIndexDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void InsertIndexDtoIsBaseDto()
        {
            var sut = new InsertIndexDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<CommonBaseDto>(sut);
        }

        [Fact]
        public void InsertIndexDtoHasFieldListDefined()
        {
            var expected = new[] {
                "Id"
            };
            var sut = new InsertIndexDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Equal(expected.Length, fields.Count);
        }

        [Theory]
        [InlineData("Id")]
        public void InsertIndexDtoHasExpectedFieldDefined(string name)
        {
            var sut = new InsertIndexDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Fact]
        public void InsertIndexDtoCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Theory]
        [InlineData(0)]
        public void InsertIndexDtoCanReadByIndex(int position)
        {
            var sut = new InsertIndexDto();
            var fields = sut.FieldList;
            var fieldName = fields[position];
            var actual = sut[position];
            var expected = sut[fieldName];
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0)]
        public void InsertIndexDtoCanWriteByIndex(int position)
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