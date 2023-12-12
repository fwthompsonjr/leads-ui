using Bogus;
using Bogus.DataSets;
using legallead.content.attr;
using legallead.content.entities;
using legallead.content.extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.content.tests.extensions
{
    public class CommonDtoExtensionsTests
    {
        private readonly Faker<MyTestDto> faker =
            new Faker<MyTestDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
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

        [DbTable(TableName ="MYTEST")]
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
                        Id = ChangeType<string>(value) ?? string.Empty;
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
