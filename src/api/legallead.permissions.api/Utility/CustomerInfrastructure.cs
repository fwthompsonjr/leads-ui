using legallead.jdbc.entities;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Interfaces;
using Stripe;

namespace legallead.permissions.api.Utility
{
    public class CustomerInfrastructure : ICustomerInfrastructure
    {
        private readonly CustomerService _customerService;
        private readonly string _paymentEnvironment;
        private readonly ICustomerRepository _repo;
        private readonly IUserRepository _db;
        public CustomerInfrastructure(StripeKeyEntity stripeKey,
            IUserRepository db, ICustomerRepository repository)
        {
            _customerService = new();
            _paymentEnvironment = stripeKey.ActiveName;
            _db = db;
            _repo = repository;
        }

        public async Task<PaymentCustomerBo?> CreateCustomer(string userId, string accountId)
        {
            var user = await _db.GetById(userId);
            if (user == null) { return null; }
            var response = await _repo.AddCustomer(new() { AccountType = _paymentEnvironment, CustomerId = accountId, UserId = userId });
            if (!response.Key) return null;
            var customer = await _repo.GetCustomer(new() { AccountType = _paymentEnvironment, UserId = userId });
            return customer;
        }

        public async Task<PaymentCustomerBo?> GetCustomer(string userId)
        {
            var customer = await GetOrCreateCustomer(userId);
            return customer;
        }

        public async Task<PaymentCustomerBo?> GetOrCreateCustomer(string userId)
        {
            var user = await _db.GetById(userId);
            if (user == null) { return null; }
            var search = new PaymentCustomerQuery { AccountType = _paymentEnvironment, UserId = userId };
            var isCreated = await _repo.DoesCustomerExist(search);
            if (isCreated)
            {
                var customer = await _repo.GetCustomer(search);
                return customer;
            }
            var stripeCustomer = await _customerService.CreateAsync(GenerateCreateOption(user.Email));
            var creation = await CreateCustomer(user.Id, stripeCustomer.Id);
            return creation;
        }

        public async Task<List<UnMappedCustomerBo>?> GetUnMappedCustomers()
        {
            var response = await _repo.GetUnMappedCustomers(new() { AccountType = _paymentEnvironment });
            return response;
        }

        public async Task<bool> MapCustomers()
        {
            var customers = await GetUnMappedCustomers();
            if (customers == null || !customers.Any()) { return true; }
            try
            {
                customers.ForEach(async customer =>
                    {
                        if (!string.IsNullOrEmpty(customer.Id))
                        {
                            _ = await GetOrCreateCustomer(customer.Id); 
                        }
                    });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static CustomerCreateOptions GenerateCreateOption(string email)
        {
            return new CustomerCreateOptions { 
                Description = "Legal Lead Customer", 
                Email = email };
        }
    }
}
