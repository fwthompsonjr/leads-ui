﻿using Bogus;
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
        private static readonly Faker<CustomerDto> faker =
            new Faker<CustomerDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserName, y => y.Hacker.Phrase())
            .RuleFor(x => x.Email, y => y.Person.Email);

        private static readonly Faker<LevelRequestDto> rqfaker =
            new Faker<LevelRequestDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExternalId, y => y.Hacker.Phrase())
            .RuleFor(x => x.InvoiceUri, y => y.Hacker.Phrase())
            .RuleFor(x => x.LevelName, y => y.Hacker.Phrase())
            .RuleFor(x => x.SessionId, y => y.Hacker.Phrase())
            .RuleFor(x => x.IsPaymentSuccess, y => y.Random.Bool())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

#pragma warning disable CS8604 // Possible null reference argument.

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
        public async Task RepoCanGetUnMappedCustomersHappyPath()
        {
            List<CustomerDto>? completion = faker.Generate(6);
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.QueryAsync<CustomerDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.GetUnMappedCustomers(new());
            Assert.NotNull(response);
        }


        [Fact]
        public async Task RepoCanGetUnMappedCustomersNoResponse()
        {
            List<CustomerDto>? completion = default;
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.QueryAsync<CustomerDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.GetUnMappedCustomers(new());
            Assert.Null(response);
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

        [Fact]
        public async Task RepoCanGetUnMappedCustomersExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.QueryAsync<CustomerDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.GetUnMappedCustomers(new());
            Assert.Null(response);
        }

        [Fact]
        public async Task RepoCanGetLevelRequestHistoryHappyPath()
        {
            List<LevelRequestDto>? completion = rqfaker.Generate(6);
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.QueryAsync<LevelRequestDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.GetLevelRequestHistory("");
            Assert.NotNull(response);
        }

        [Fact]
        public async Task RepoCanGetLevelRequestHistoryNoResponse()
        {
            List<LevelRequestDto>? completion = default;
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.QueryAsync<LevelRequestDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.GetLevelRequestHistory("");
            Assert.Null(response);
        }

        [Fact]
        public async Task RepoCanGetLevelRequestHistoryExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.QueryAsync<LevelRequestDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.GetLevelRequestHistory("");
            Assert.Null(response);
        }

        [Fact]
        public async Task RepoCanAddLevelChangeRequestHappyPath()
        {
            var completion = Task.CompletedTask;
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .Returns(completion);
            var response = await service.AddLevelChangeRequest("");
            Assert.True(response.Key);
        }

        [Fact]
        public async Task RepoCanAddLevelChangeRequestExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.AddLevelChangeRequest("");
            Assert.False(response.Key);
        }


        [Fact]
        public async Task RepoCanGetLevelRequestByIdHappyPath()
        {
            var completion = rqfaker.Generate();
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<LevelRequestDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.GetLevelRequestById("");
            Assert.NotNull(response);
        }

        [Fact]
        public async Task RepoCanGetLevelRequestByIdNoResponse()
        {
            LevelRequestDto? completion = default;
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<LevelRequestDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ReturnsAsync(completion);
            var response = await service.GetLevelRequestById("");
            Assert.Null(response);
        }

        [Fact]
        public async Task RepoCanGetLevelRequestByIdExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.QuerySingleOrDefaultAsync<LevelRequestDto>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.GetLevelRequestById("");
            Assert.Null(response);
        }

        [Fact]
        public async Task RepoCanUpdateLevelChangeRequestHappyPath()
        {
            var completion = Task.CompletedTask;
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .Returns(completion);
            var response = await service.UpdateLevelChangeRequest("");
            Assert.True(response.Key);
        }

        [Fact]
        public async Task RepoCanUpdateLevelChangeRequestExceptionPath()
        {
            var completion = new Faker().System.Exception();
            var provider = new CustomerRepoContainer();
            var mock = provider.CommandMock;
            var service = provider.CustomerRepo;
            mock.Setup(m => m.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<DynamicParameters>()))
                .ThrowsAsync(completion);
            var response = await service.UpdateLevelChangeRequest("");
            Assert.False(response.Key);
        }

#pragma warning restore CS8604 // Possible null reference argument.

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