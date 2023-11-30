using Bogus;
using legallead.permissions.api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace permissions.api.tests.Models
{
    public class ApplicationModelTests
    {
        private static readonly Faker<ApplicationModel> faker =
            new Faker<ApplicationModel>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Name, y => y.Person.FullName);

        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ApplicationModel();
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
            Assert.False(string.IsNullOrEmpty(test.Id));
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
    }
}
