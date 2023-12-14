using Bogus;
using Castle.Core.Internal;
using legallead.content.entities;
using legallead.content.extensions;
using System.ComponentModel.DataAnnotations;

namespace legallead.content.tests.entities
{
    public class CreateContentRequestTests
    {
        private readonly Faker<CreateContentRequest> faker =
            new Faker<CreateContentRequest>()
            .RuleFor(x => x.Name, y => y.Company.CompanyName());

        [Fact]
        public void CreateContentRequestCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new CreateContentRequest();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void CreateContentRequestHasMaxLengthAttribute()
        {
            const int expected = 500;
            var sut = faker.Generate();
            var attribute = sut.GetType().GetProperty("Name")?.GetAttribute<StringLengthAttribute>();
            var maxlength = attribute?.MaximumLength ?? 0;
            Assert.Equal(expected, maxlength);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(50)]
        [InlineData(500)]
        [InlineData(1000)]
        public void CreateContentRequestCanVaildateMaxLengthAttribute(int length)
        {
            bool expected = length <= 500;
            var text = new Faker().Random.AlphaNumeric(length);
            var sut = faker.Generate();
            sut.Name = text;
            _ = sut.GetValidationResult(out var isvalid);
            Assert.Equal(expected, isvalid);
        }

        [Fact]
        public void CreateContentRequestCanUpdateName()
        {
            var items = faker.Generate(2);
            items[0].Name = items[1].Name;
            Assert.Equal(items[1].Name, items[0].Name);
        }
    }
}