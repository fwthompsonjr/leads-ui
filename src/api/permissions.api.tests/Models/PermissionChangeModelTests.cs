using legallead.permissions.api.Models;

namespace permissions.api.tests.Models
{
    public class PermissionChangeModelTests
    {
        private readonly Faker<PermissionChangeModel> faker =
            new Faker<PermissionChangeModel>()
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.Request, y => y.Company.CompanyName())
            .RuleFor(x => x.Email, y => y.Company.CompanyName());

        [Fact]
        public void RequestCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PermissionChangeModelCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new PermissionChangeModel();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PermissionChangeModelCanUpdateName()
        {
            var items = faker.Generate(2);
            items[0].Name = items[1].Name;
            Assert.Equal(items[1].Name, items[0].Name);
        }

        [Fact]
        public void PermissionChangeModelCanUpdateEmail()
        {
            var items = faker.Generate(2);
            items[0].Email = items[1].Email;
            Assert.Equal(items[1].Email, items[0].Email);
        }

        [Fact]
        public void PermissionChangeModelCanUpdateRequest()
        {
            var items = faker.Generate(2);
            items[0].Request = items[1].Request;
            Assert.Equal(items[1].Request, items[0].Request);
        }

        [Fact]
        public void DiscountRequestCanUpdateDto()
        {
            var items = faker.Generate(2);
            items[0].Dto = items[1].Dto;
            Assert.Equal(items[1].Dto, items[0].Dto);
        }
    }
}