using Bogus;
using legallead.logging.attr;
using legallead.logging.entities;
using legallead.logging.extensions;

namespace legallead.logging.tests.extensions
{
    public class CommonDtoExtensionsTests
    {
        private const int seed = 100;

        private readonly Faker<MyTestDto> faker =
            new Faker<MyTestDto>()
            .RuleFor(x => x.Id, y => y.IndexFaker + seed)
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(25));

        [Fact]
        public void DtoHasTableName()
        {
            var sut = faker.Generate();
            Assert.Equal("MYTEST", sut.TableName);
        }

        [Fact]
        public void DtoCanGetSelectParameters()
        {
            var sut = faker.Generate();
            var actual = sut.SelectParameters(sut);
            Assert.NotNull(actual);
            Assert.NotNull(actual.ParameterNames);
            Assert.Contains("Name", actual.ParameterNames);
        }

        [Fact]
        public void DtoCanGetUpdateParameters()
        {
            var sut = faker.Generate();
            var actual = sut.UpdateParameters();
            Assert.NotNull(actual);
            Assert.NotNull(actual.ParameterNames);
            Assert.Contains("Id", actual.ParameterNames);
        }

        [Fact]
        public void DtoCanGetInsertParameters()
        {
            var sut = faker.Generate();
            var actual = sut.InsertParameters();
            Assert.NotNull(actual);
            Assert.NotNull(actual.ParameterNames);
            Assert.Contains("Id", actual.ParameterNames);
        }

        [Fact]
        public void DtoCanGetDeleteParameters()
        {
            var sut = faker.Generate();
            var actual = sut.DeleteParameters();
            Assert.NotNull(actual);
            Assert.NotNull(actual.ParameterNames);
            Assert.Contains("Id", actual.ParameterNames);
        }

        [Fact]
        public void DtoCanGetSelectSQL()
        {
            var sut = faker.Generate();
            sut.Name = null;
            var actual = sut.SelectSQL(sut);
            Assert.False(string.IsNullOrEmpty(actual));
            Assert.Contains("Id", actual);
        }

        [Fact]
        public void DtoCanGetAllFieldSelectSQL()
        {
            var sut = faker.Generate();
            var actual = sut.SelectSQL();
            Assert.False(string.IsNullOrEmpty(actual));
            Assert.Contains("*", actual);
        }

        [Fact]
        public void DtoCanGetInsertSQL()
        {
            var sut = faker.Generate();
            var actual = sut.InsertSQL();
            Assert.False(string.IsNullOrEmpty(actual));
            Assert.Contains("Id", actual);
        }

        [Fact]
        public void DtoCanGetUpdateByIdSQL()
        {
            var sut = faker.Generate();
            var actual = sut.UpdateByIdSQL();
            Assert.False(string.IsNullOrEmpty(actual));
            Assert.Contains("Id", actual);
        }

        [Fact]
        public void DtoCanGetUpdateByIdSQLWithPredicate()
        {
            var sut = faker.Generate();
            var query = faker.Generate();
            var actual = sut.UpdateByIdSQL(query);
            Assert.False(string.IsNullOrEmpty(actual));
            Assert.Contains("@Name", actual);
        }

        [Fact]
        public void DtoCanGetDeleteSQL()
        {
            var sut = faker.Generate();
            var actual = sut.DeleteSQL();
            Assert.False(string.IsNullOrEmpty(actual));
            Assert.Contains("Id", actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(21)]
        public void DtoIndexerGetNotInRangeReturnsNull(int fieldId)
        {
            var sut = faker.Generate();
            var actual = sut[fieldId];
            Assert.Null(actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(21)]
        public void DtoIndexerSetNotInRangeDoesNotError(int fieldId)
        {
            var exception = Record.Exception(() =>
            {
                var sut = faker.Generate();
                var temp = faker.Generate().Name;
                sut[fieldId] = temp;
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void DtoIndexerSetDoesNotError(int fieldId)
        {
            var exception = Record.Exception(() =>
            {
                var sut = faker.Generate();
                var temp = faker.Generate()[fieldId];
                sut[fieldId] = temp;
            });
            Assert.Null(exception);
        }

        [LogTable(TableName = "MYTEST")]
        private sealed class MyTestDto : CommonBaseDto
        {
            public string? Name { get; set; }

            public override object? this[string field]
            {
                get
                {
                    if (field == null) return null;
                    var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                    if (fieldName == null) return null;
                    if (fieldName.Equals("Id", Comparison)) return Id;
                    if (fieldName.Equals("Name", Comparison)) return Name;
                    return null;
                }
                set
                {
                    if (field == null) return;
                    var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                    if (fieldName == null) return;
                    if (fieldName.Equals("Id", Comparison))
                    {
                        Id = ChangeType<long>(value);
                        return;
                    }
                    if (fieldName.Equals("Name", Comparison))
                    {
                        Name = ChangeType<string>(value);
                    }
                }
            }
        }
    }
}