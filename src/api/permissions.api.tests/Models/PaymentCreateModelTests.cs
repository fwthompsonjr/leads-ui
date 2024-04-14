using legallead.jdbc.entities;
using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Http;

namespace permissions.api.tests.Models
{
    public class PaymentCreateModelTests
    {
        [Fact]
        public void ItemCanBeCreated()
        {
            var sut = GetModel();
            Assert.NotNull(sut);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ItemCanGetProperties(int fieldId)
        {
            var sut = GetModel();
            if (fieldId == 0) Assert.NotNull(sut.SuccessUrlFormat);
            if (fieldId == 1) Assert.NotNull(sut.SearchId);
            if (fieldId == 2) Assert.NotNull(sut.ProductType);
            if (fieldId == 3) Assert.NotNull(sut.CurrentUser);
        }
        private static readonly Faker<User> userfaker = new Faker<User>()
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());

        private static PaymentCreateModel GetModel()
        {
            var faker = new Faker();
            return new PaymentCreateModel(
                GetRequest(),
                userfaker.Generate(),
                faker.Random.AlphaNumeric(10),
                faker.Random.AlphaNumeric(5));
        }

        private static HttpRequest GetRequest()
        {
            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Scheme).Returns("http");
            request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
            request.Setup(x => x.PathBase).Returns(PathString.FromUriComponent("/api"));
            return request.Object;
        }
    }
}
