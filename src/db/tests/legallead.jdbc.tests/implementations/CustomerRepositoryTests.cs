using Bogus;
using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;
using System.Data;

namespace legallead.jdbc.tests.implementations
{
    public class CustomerRepositoryTests
    {
        [Fact]
        public void RepoCanBeConstructed()
        {
            var provider = new CustomerRepoContainer();
            var repo = provider.CustomerRepo;
            Assert.NotNull(repo);
        }

        [Fact]
        public async Task RepoDoesCustomerExistHappyPath()
        {
            var completion = new PaymentCustomerDto { Id = Guid.NewGuid().ToString("D") };
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<PaymentCustomerDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.DoesCustomerExist(new());
            Assert.True(response);
        }

        [Fact]
        public async Task RepoDoesCustomerExistNoResponse()
        {
            PaymentCustomerDto? completion = default;
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<PaymentCustomerDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.DoesCustomerExist(new());
            Assert.False(response);
        }

        [Fact]
        public async Task RepoDoesCustomerExistExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<PaymentCustomerDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.DoesCustomerExist(new());
            Assert.False(response);
        }

        [Fact]
        public async Task RepoCanGetCustomerHappyPath()
        {
            var completion = new PaymentCustomerDto { Id = Guid.NewGuid().ToString("D") };
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<PaymentCustomerDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.GetCustomer(new());
            Assert.NotNull(response);
        }

        [Fact]
        public async Task RepoCanGetCustomerNoResponse()
        {
            PaymentCustomerDto? completion = default;
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<PaymentCustomerDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.GetCustomer(new());
            Assert.Null(response);
        }

        [Fact]
        public async Task RepoCanGetCustomerExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<PaymentCustomerDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.GetCustomer(new());
            Assert.Null(response);
        }

        [Fact]
        public async Task RepoCanAddCustomerHappyPath()
        {
            var completion = Task.CompletedTask;
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .Returns(completion);
            var response = await service.AddCustomer(new());
            Assert.True(response.Key);
        }

        [Fact]
        public async Task RepoCanAddCustomerExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.AddCustomer(new());
            Assert.False(response.Key);
        }

        private sealed class CustomerRepoContainer
        {
            private readonly ICustomerRepository repo;
            private readonly Mock<IDapperCommand> command;
            public CustomerRepoContainer()
            {

                command = new Mock<IDapperCommand>();
                var dataContext = new DataContext(command.Object);
                repo = new CustomerRepository(dataContext);
            }

            public ICustomerRepository CustomerRepo => repo;
            public Mock<IDapperCommand> CommandMock => command;
        }
    }
}
