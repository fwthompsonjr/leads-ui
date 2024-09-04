using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace permissions.api.tests.Contollers
{
    public class ApplicationControllerTests : BaseControllerTest
    {
        [Fact]
        public void ControllerCanBeConstructed()
        {
            var sut = GetProvider().GetRequiredService<ApplicationController>();
            Assert.NotNull(sut);
        }

        [Fact]
        public void ControllerCanDeliverReadMe()
        {
            var sut = GetProvider().GetRequiredService<ApplicationController>();
            var actual = sut.ReadMe();
            var additional = sut.ReadMe();
            Assert.False(string.IsNullOrEmpty(actual));
            Assert.Equal(additional, actual);
        }

        [Fact]
        public async Task ControllerCanListAppsAsync()
        {
            var faker = new Faker<Component>()
                .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
                .RuleFor(x => x.Name, y => y.Person.FirstName);
            var provider = GetProvider();
            var componentdb = provider.GetRequiredService<Mock<IComponentRepository>>();
            var sut = provider.GetRequiredService<ApplicationController>();
            var expected = faker.Generate(4).AsEnumerable();
            componentdb.Setup(c => c.GetAll()).ReturnsAsync(expected);
            var actual = await sut.ListAsync();
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }

        [Fact]
        public async Task ControllerCanListAppsFromNoResponseAsync()
        {
            var faker = new Faker<Component>()
                .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
                .RuleFor(x => x.Name, y => y.Person.FirstName);
            var provider = GetProvider();
            var componentdb = provider.GetRequiredService<Mock<IComponentRepository>>();
            var sut = provider.GetRequiredService<ApplicationController>();
            var expected = faker.Generate(0).AsEnumerable();
            componentdb.Setup(c => c.GetAll()).ReturnsAsync(expected);
            var actual = await sut.ListAsync();
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }

        [Fact]
        public async Task ControllerCanListAppsFromNoNamesAsync()
        {
            var faker = new Faker<Component>()
                .RuleFor(x => x.Id, y => y.Random.Guid().ToString());
            var provider = GetProvider();
            var componentdb = provider.GetRequiredService<Mock<IComponentRepository>>();
            var sut = provider.GetRequiredService<ApplicationController>();
            var expected = faker.Generate(5).AsEnumerable();
            componentdb.Setup(c => c.GetAll()).ReturnsAsync(expected);
            var actual = await sut.ListAsync();
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }


        [Fact]
        public async Task ControllerCanListAppsFromExceptionAsync()
        {
            var faker = new Faker();
            var provider = GetProvider();
            var componentdb = provider.GetRequiredService<Mock<IComponentRepository>>();
            var sut = provider.GetRequiredService<ApplicationController>();
            var expected = faker.System.Exception();
            componentdb.Setup(c => c.GetAll()).ThrowsAsync(expected);
            var actual = await sut.ListAsync();
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }

        [Fact]
        public void ControllerCanListStates()
        {
            var provider = GetProvider();
            var sut = provider.GetRequiredService<ApplicationController>();
            var actual = sut.StateList();
            Assert.NotNull(actual);
        }

        [Fact]
        public async Task ControllerRegistrationRequiresEmailAsync()
        {
            var faker = new Faker<RegisterAccountModel>()
                .RuleFor(x => x.UserName, y => y.Random.Guid().ToString())
                .RuleFor(x => x.Password, y => y.Random.AlphaNumeric(16))
                .RuleFor(x => x.Email, y => y.Person.Email);

            var provider = GetProvider();
            var request = faker.Generate();
            var sut = provider.GetRequiredService<ApplicationController>();
            request.Email = string.Empty;
            var actual = await sut.RegisterAsync(request);
            Assert.NotNull(actual);
            if (actual is not BadRequestObjectResult oresult)
            {
                Assert.Fail($"Invalid controller response. Expected BadRequestObjectResult, Actual {actual.GetType().Name}");
                return;
            }

            Assert.NotNull(oresult.Value);
            var message = Convert.ToString(oresult.Value) ?? string.Empty;
            Assert.False(string.IsNullOrWhiteSpace(message));
            Assert.False(Guid.TryParse(message, out var _));
        }

        [Fact]
        public async Task ControllerRegistrationRequiresPasswordAsync()
        {
            var faker = new Faker<RegisterAccountModel>()
                .RuleFor(x => x.UserName, y => y.Random.Guid().ToString())
                .RuleFor(x => x.Password, y => y.Random.AlphaNumeric(16))
                .RuleFor(x => x.Email, y => y.Person.Email);

            var provider = GetProvider();
            var request = faker.Generate();
            var sut = provider.GetRequiredService<ApplicationController>();
            request.Password = string.Empty;
            var actual = await sut.RegisterAsync(request);
            Assert.NotNull(actual);
            if (actual is not BadRequestObjectResult oresult)
            {
                Assert.Fail($"Invalid controller response. Expected BadRequestObjectResult, Actual {actual.GetType().Name}");
                return;
            }

            Assert.NotNull(oresult.Value);
            Assert.False(string.IsNullOrEmpty(oresult.Value.ToString()));
        }

        [Fact]
        public async Task ControllerRegistrationRequiresUserNameAsync()
        {
            var faker = new Faker<RegisterAccountModel>()
                .RuleFor(x => x.UserName, y => y.Random.Guid().ToString())
                .RuleFor(x => x.Password, y => y.Random.AlphaNumeric(16))
                .RuleFor(x => x.Email, y => y.Person.Email);

            var provider = GetProvider();
            var request = faker.Generate();
            var sut = provider.GetRequiredService<ApplicationController>();
            request.UserName = string.Empty;
            var actual = await sut.RegisterAsync(request);
            Assert.NotNull(actual);
            if (actual is not BadRequestObjectResult oresult)
            {
                Assert.Fail($"Invalid controller response. Expected BadRequestObjectResult, Actual {actual.GetType().Name}");
                return;
            }

            Assert.NotNull(oresult.Value);
            var message = Convert.ToString(oresult.Value) ?? string.Empty;
            Assert.False(string.IsNullOrWhiteSpace(message));
            Assert.False(Guid.TryParse(message, out var _));
        }

        [Fact]
        public async Task ControllerRegistrationRoundTripAsync()
        {
            var faker = new Faker<RegisterAccountModel>()
                .RuleFor(x => x.UserName, y => y.Random.Guid().ToString())
                .RuleFor(x => x.Password, y => y.Random.AlphaNumeric(8))
                .RuleFor(x => x.Email, y => y.Person.Email);

            var provider = GetProvider();
            var request = faker.Generate();
            request.Password = $"135Aa-{request.Password}-Bb531";
            var sut = provider.GetRequiredService<ApplicationController>();
            var actual = await sut.RegisterAsync(request);
            Assert.NotNull(actual);
        }
    }
}