using Bogus;
using legallead.permissions.api;
using legallead.permissions.api.Model;

namespace permissions.api.tests.Models
{
    public class ApplicationRequestModelTests
    {
        private static readonly Faker<ApplicationRequestModel> faker =
            new Faker<ApplicationRequestModel>()
            .RuleFor(x => x.Id, y => Guid.NewGuid())
            .RuleFor(x => x.Name, y => y.Person.FullName);

        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ApplicationRequestModel();
            });
            Assert.Null(exception);
        }
        [Fact]
        public void ModelCanGenerate()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanGetId()
        {
            var test = faker.Generate();
            Assert.NotNull(test.Id);
        }

        [Fact]
        public void ModelCanSetId()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.Id = source.Id;
            Assert.Equal(source.Id, test.Id);
        }

        [Fact]
        public void ModelCanGetName()
        {
            var test = faker.Generate();
            Assert.False(string.IsNullOrEmpty(test.Name));
        }

        [Fact]
        public void ModelCanSetName()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.Name = source.Name;
            Assert.Equal(source.Name, test.Name);
        }

        [Fact]
        public void ModelIsInValidWithEmptyName()
        {
            var test = faker.Generate();
            test.Name = string.Empty;
            var results = test.Validate(out bool isValid);
            Assert.False(isValid);
            Assert.NotEmpty(results);
            var errors = results.Select(x => x.ErrorMessage);
            Assert.Contains("Name is a required field.", errors);
        }
        [Fact]
        public void ModelIsInValidWithNullId()
        {
            var test = faker.Generate();
            test.Id = null;
            var results = test.Validate(out bool isValid);
            Assert.False(isValid);
            Assert.NotEmpty(results);
            var errors = results.Select(x => x.ErrorMessage);
            Assert.Contains("Id is a required field.", errors);
        }
    }
}