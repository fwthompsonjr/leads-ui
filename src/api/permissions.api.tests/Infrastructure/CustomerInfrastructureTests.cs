using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;
using legallead.permissions.api.Utility;
using Microsoft.Extensions.DependencyInjection;
using Stripe;

namespace permissions.api.tests.Infrastructure
{
    public class CustomerInfrastructureTests
    {
        [Theory]
        [InlineData(typeof(CustomerService))]
        [InlineData(typeof(StripeKeyEntity))]
        [InlineData(typeof(IUserRepository))]
        [InlineData(typeof(ICustomerRepository))]
        [InlineData(typeof(ISubscriptionInfrastructure))]
        [InlineData(typeof(ICustomerInfrastructure))]
        [InlineData(typeof(CustomerInfrastructure))]
        public void MockProviderCanGetTypes(Type type)
        {
            var provider = GetProvider();
            var service = provider.GetService(type);
            Assert.NotNull(service);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public async Task ServiceCanCreateCustomerAsync(
            bool doesUserExist,
            bool isCustomerAdded)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var user = doesUserExist ? fakeUser.Generate() : null;
                var insertResponse = new KeyValuePair<bool, string>(isCustomerAdded, "Unit test mock execution");
                var getCustomerResponse = new PaymentCustomerBo();
                var parms = new CreateCustomerParameters();
                var provider = GetProvider();

                var userDb = provider.GetRequiredService<Mock<IUserRepository>>();
                var custDb = provider.GetRequiredService<Mock<ICustomerRepository>>();

                userDb.Setup(s => s.GetById(It.IsAny<string>())).ReturnsAsync(user);
                custDb.Setup(s => s.AddCustomer(It.IsAny<PaymentCustomerInsert>())).ReturnsAsync(insertResponse);
                custDb.Setup(s => s.GetCustomer(It.IsAny<PaymentCustomerQuery>())).ReturnsAsync(getCustomerResponse);

                var service = provider.GetRequiredService<ICustomerInfrastructure>();
                _ = await service.CreateCustomerAsync(parms.UserId, parms.AccountId);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true, false, true)] // happy path
        [InlineData(false, false, true)] // user is not found
        [InlineData(true, true, true)] // user account is alreay created
        public async Task ServiceCanGetOrCreateCustomerAsync(
            bool doesUserExist,
            bool doesCustomerExist,
            bool isCustomerAdded)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var user = doesUserExist ? fakeUser.Generate() : null;
                var insertResponse = new KeyValuePair<bool, string>(isCustomerAdded, "Unit test mock execution");
                var getCustomerResponse = new PaymentCustomerBo();
                var parms = new CreateCustomerParameters();
                var provider = GetProvider();

                var userDb = provider.GetRequiredService<Mock<IUserRepository>>();
                var custDb = provider.GetRequiredService<Mock<ICustomerRepository>>();

                userDb.Setup(s => s.GetById(It.IsAny<string>())).ReturnsAsync(user);
                custDb.Setup(s => s.DoesCustomerExist(It.IsAny<PaymentCustomerQuery>())).ReturnsAsync(doesCustomerExist);
                custDb.Setup(s => s.AddCustomer(It.IsAny<PaymentCustomerInsert>())).ReturnsAsync(insertResponse);
                custDb.Setup(s => s.GetCustomer(It.IsAny<PaymentCustomerQuery>())).ReturnsAsync(getCustomerResponse);

                var service = provider.GetRequiredService<ICustomerInfrastructure>();
                _ = await service.GetOrCreateCustomerAsync(parms.UserId);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true, false, true)] // happy path
        [InlineData(false, false, true)] // user is not found
        [InlineData(true, true, true)] // user account is alreay created
        public async Task ServiceCanGetCustomerAsync(
            bool doesUserExist,
            bool doesCustomerExist,
            bool isCustomerAdded)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var user = doesUserExist ? fakeUser.Generate() : null;
                var insertResponse = new KeyValuePair<bool, string>(isCustomerAdded, "Unit test mock execution");
                var getCustomerResponse = new PaymentCustomerBo();
                var parms = new CreateCustomerParameters();
                var provider = GetProvider();

                var userDb = provider.GetRequiredService<Mock<IUserRepository>>();
                var custDb = provider.GetRequiredService<Mock<ICustomerRepository>>();

                userDb.Setup(s => s.GetById(It.IsAny<string>())).ReturnsAsync(user);
                custDb.Setup(s => s.DoesCustomerExist(It.IsAny<PaymentCustomerQuery>())).ReturnsAsync(doesCustomerExist);
                custDb.Setup(s => s.AddCustomer(It.IsAny<PaymentCustomerInsert>())).ReturnsAsync(insertResponse);
                custDb.Setup(s => s.GetCustomer(It.IsAny<PaymentCustomerQuery>())).ReturnsAsync(getCustomerResponse);

                var service = provider.GetRequiredService<ICustomerInfrastructure>();
                _ = await service.GetCustomerAsync(parms.UserId);
            });
            Assert.Null(exception);
        }

        [Fact]
        public async Task ServiceCanGetUnMappedCustomersAsync()
        {
            var customers = fakeUnMapCustomer.Generate(10);
            var provider = GetProvider();
            var custDb = provider.GetRequiredService<Mock<ICustomerRepository>>();

            custDb.Setup(s => s.GetUnMappedCustomers(It.IsAny<PaymentCustomerQuery>())).ReturnsAsync(customers);
            var service = provider.GetRequiredService<ICustomerInfrastructure>();
            var response = await service.GetUnMappedCustomersAsync();
            Assert.NotNull(response);
        }
        [Theory]
        [InlineData(true, false, true)] // happy path
        [InlineData(false, false, true)] // user is not found
        [InlineData(true, true, true)] // user account is alreay created
        [InlineData(true, false, true, null)]
        [InlineData(true, false, true, 0)]
        [InlineData(true, false, true, 5, true)]
        public async Task ServiceCanMapCustomersAsync(
            bool doesUserExist,
            bool doesCustomerExist,
            bool isCustomerAdded,
            int? customerCount = 10,
            bool throwsException = false
            )
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var error = new Faker().System.Exception();
                var user = doesUserExist ? fakeUser.Generate() : null;
                var customers = !customerCount.HasValue ? null : fakeUnMapCustomer.Generate(customerCount.Value);
                var insertResponse = new KeyValuePair<bool, string>(isCustomerAdded, "Unit test mock execution");
                var getCustomerResponse = new PaymentCustomerBo();
                var provider = GetProvider();

                var userDb = provider.GetRequiredService<Mock<IUserRepository>>();
                var custDb = provider.GetRequiredService<Mock<ICustomerRepository>>();

                userDb.Setup(s => s.GetById(It.IsAny<string>())).ReturnsAsync(user);
                custDb.Setup(s => s.DoesCustomerExist(It.IsAny<PaymentCustomerQuery>())).ReturnsAsync(doesCustomerExist);
                custDb.Setup(s => s.AddCustomer(It.IsAny<PaymentCustomerInsert>())).ReturnsAsync(insertResponse);
                custDb.Setup(s => s.GetCustomer(It.IsAny<PaymentCustomerQuery>())).ReturnsAsync(getCustomerResponse);

                if (throwsException)
                    custDb.Setup(s => s.GetUnMappedCustomers(It.IsAny<PaymentCustomerQuery>())).ThrowsAsync(error);
                else
                    custDb.Setup(s => s.GetUnMappedCustomers(It.IsAny<PaymentCustomerQuery>())).ReturnsAsync(customers);
                var service = provider.GetRequiredService<ICustomerInfrastructure>();
                _ = await service.MapCustomersAsync();
            });
            Assert.Null(exception);
        }

        [Fact]
        public async Task ServiceCanAddLevelChangeRequestAsync()
        {
            var request = new LevelChangeRequest { ExternalId = "123456", LevelName = "Test" };
            var insertResponse = new KeyValuePair<bool, string>(true, "Unit test mock execution");
            var findLevelResponse = new LevelRequestBo();
            var provider = GetProvider();
            var custDb = provider.GetRequiredService<Mock<ICustomerRepository>>();

            custDb.Setup(m => m.AddLevelChangeRequest(It.IsAny<string>())).ReturnsAsync(insertResponse);
            custDb.Setup(m => m.GetLevelRequestById(It.IsAny<string>())).ReturnsAsync(findLevelResponse);
            var service = provider.GetRequiredService<ICustomerInfrastructure>();
            var response = await service.AddLevelChangeRequestAsync(request);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task ServiceCanGetLevelRequestByIdAsync()
        {
            var externalId = "123456";
            var findLevelResponse = new LevelRequestBo();
            var provider = GetProvider();
            var custDb = provider.GetRequiredService<Mock<ICustomerRepository>>();

            custDb.Setup(m => m.GetLevelRequestById(It.IsAny<string>())).ReturnsAsync(findLevelResponse);
            var service = provider.GetRequiredService<ICustomerInfrastructure>();
            var response = await service.GetLevelRequestByIdAsync(externalId);
            Assert.NotNull(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("12345")]
        public async Task ServiceCanCompleteLevelRequestAsync(string? externalId)
        {
            var request = new LevelRequestBo { ExternalId = externalId };
            var updateResponse = new KeyValuePair<bool, string>(true, "Unit test mock execution");
            var findLevelResponse = new LevelRequestBo();
            var provider = GetProvider();
            var custDb = provider.GetRequiredService<Mock<ICustomerRepository>>();

            custDb.Setup(m => m.UpdateLevelChangeRequest(It.IsAny<string>())).ReturnsAsync(updateResponse);
            custDb.Setup(m => m.GetLevelRequestById(It.IsAny<string>())).ReturnsAsync(findLevelResponse);
            var service = provider.GetRequiredService<ICustomerInfrastructure>();
            var response = await service.CompleteLevelRequestAsync(request);
            if (string.IsNullOrEmpty(externalId)) Assert.Null(response);
            else Assert.NotNull(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("12345")]
        public async Task ServiceCanCompleteDiscountRequestAsync(string? externalId)
        {
            var request = new LevelRequestBo { ExternalId = externalId };
            var updateResponse = new KeyValuePair<bool, string>(true, "Unit test mock execution");
            var findLevelResponse = new LevelRequestBo();
            var provider = GetProvider();
            var custDb = provider.GetRequiredService<Mock<ICustomerRepository>>();

            custDb.Setup(m => m.UpdateDiscountChangeRequest(It.IsAny<string>())).ReturnsAsync(updateResponse);
            custDb.Setup(m => m.GetDiscountRequestById(It.IsAny<string>())).ReturnsAsync(findLevelResponse);
            var service = provider.GetRequiredService<ICustomerInfrastructure>();
            var response = await service.CompleteDiscountRequestAsync(request);
            if (string.IsNullOrEmpty(externalId)) Assert.Null(response);
            else Assert.NotNull(response);
        }

        [Fact]
        public async Task ServiceCanAddDiscountChangeRequestAsync()
        {
            var request = new LevelChangeRequest { ExternalId = "123456", LevelName = "Test" };
            var insertResponse = new KeyValuePair<bool, string>(true, "Unit test mock execution");
            var findLevelResponse = new LevelRequestBo();
            var provider = GetProvider();
            var custDb = provider.GetRequiredService<Mock<ICustomerRepository>>();

            custDb.Setup(m => m.AddDiscountChangeRequest(It.IsAny<string>())).ReturnsAsync(insertResponse);
            custDb.Setup(m => m.GetDiscountRequestById(It.IsAny<string>())).ReturnsAsync(findLevelResponse);
            var service = provider.GetRequiredService<ICustomerInfrastructure>();
            var response = await service.AddDiscountChangeRequestAsync(request);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task ServiceCanGetDiscountRequestByIdAsync()
        {
            var request = new LevelChangeRequest { ExternalId = "123456", LevelName = "Test" };
            var findLevelResponse = new LevelRequestBo();
            var provider = GetProvider();
            var custDb = provider.GetRequiredService<Mock<ICustomerRepository>>();

            custDb.Setup(m => m.GetDiscountRequestById(It.IsAny<string>())).ReturnsAsync(findLevelResponse);
            var service = provider.GetRequiredService<ICustomerInfrastructure>();
            var response = await service.GetDiscountRequestByIdAsync(request.ExternalId);
            Assert.NotNull(response);
        }

        private sealed class CreateCustomerParameters
        {
            private static readonly Faker faker = new();
            public string UserId { get; set; } = faker.Random.AlphaNumeric(12);
            public string AccountId { get; set; } = faker.Random.AlphaNumeric(8);
        }

        private static IServiceProvider GetProvider()
        {
            var services = new ServiceCollection();
            var stripeKey = new Mock<StripeKeyEntity>();
            var userDb = new Mock<IUserRepository>();
            var custDb = new Mock<ICustomerRepository>();
            var subDb = new Mock<ISubscriptionInfrastructure>();
            var custService = new Mock<CustomerService>();

            // add mocks
            services.AddSingleton(stripeKey);
            services.AddSingleton(userDb);
            services.AddSingleton(custDb);
            services.AddSingleton(subDb);
            services.AddSingleton(custService);

            // add implementations
            services.AddSingleton(stripeKey.Object);
            services.AddSingleton(userDb.Object);
            services.AddSingleton(custDb.Object);
            services.AddSingleton(subDb.Object);
            services.AddSingleton(custService.Object);

            // expose default implementation
            services.AddSingleton<ICustomerInfrastructure>(x =>
            {
                var infra = new CustomerInfrastructure(stripeKey.Object, userDb.Object, custDb.Object);
                var customer = x.GetRequiredService<CustomerService>();
                infra.GetCustomerService = customer;
                infra.SubscriptionInfrastructure(subDb.Object);
                return infra;
            });

            // expose internal implementation as class
            services.AddSingleton(x =>
            {
                var infra = new CustomerInfrastructure(stripeKey.Object, userDb.Object, custDb.Object, subDb.Object);
                return infra;
            });

            return services.BuildServiceProvider();
        }

        private static readonly Faker<User> fakeUser = new Faker<User>()
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());

        private static readonly Faker<UnMappedCustomerBo> fakeUnMapCustomer = new Faker<UnMappedCustomerBo>()
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());
    }
}
