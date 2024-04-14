using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;
using Newtonsoft.Json;
using Stripe;
using System.Diagnostics.CodeAnalysis;

namespace legallead.permissions.api.Utility
{
    public class CustomerInfrastructure : ICustomerInfrastructure
    {
        private readonly string _paymentEnvironment;
        private readonly ICustomerRepository _repo;
        private readonly IUserRepository _db;
        private ISubscriptionInfrastructure? _subscriptiondb;
        public CustomerInfrastructure(
            StripeKeyEntity stripeKey,
            IUserRepository db,
            ICustomerRepository repository)
        {
            GetCustomerService = new();
            _paymentEnvironment = stripeKey.ActiveName;
            _db = db;
            _repo = repository;
        }


        internal CustomerInfrastructure(
            StripeKeyEntity stripeKey,
            IUserRepository db,
            ICustomerRepository repository,
            ISubscriptionInfrastructure subscription) :
            this(stripeKey, db, repository)
        {
            _subscriptiondb = subscription;
        }

        internal CustomerService GetCustomerService { get; set; }

        internal void SubscriptionInfrastructure(ISubscriptionInfrastructure subscription)
        {
            if (_subscriptiondb != null) { return; }
            _subscriptiondb = subscription;
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
            var stripeCustomerId = await CreateStripeCustomerAndGetIndex(user.Email);
            var creation = await CreateCustomer(user.Id, stripeCustomerId);
            return creation;
        }

        public async Task<PaymentCustomerBo?> GetCustomer(string userId)
        {
            var customer = await GetOrCreateCustomer(userId);
            return customer;
        }
        public async Task<List<UnMappedCustomerBo>?> GetUnMappedCustomers()
        {
            var response = await _repo.GetUnMappedCustomers(new() { AccountType = _paymentEnvironment });
            return response;
        }

        public async Task<bool> MapCustomers()
        {
            try
            {
                var customers = await GetUnMappedCustomers();
                if (customers == null || !customers.Any()) { return true; }
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

        public async Task<LevelRequestBo?> AddLevelChangeRequest(LevelChangeRequest request)
        {
            _ = await _repo.AddLevelChangeRequest(JsonConvert.SerializeObject(request));
            var item = await _repo.GetLevelRequestById(request.ExternalId ?? string.Empty);
            return item;
        }

        public async Task<LevelRequestBo?> GetLevelRequestById(string externalId)
        {
            var item = await _repo.GetLevelRequestById(externalId);
            return item;
        }

        public async Task<LevelRequestBo?> CompleteLevelRequest(LevelRequestBo request)
        {
            if (string.IsNullOrEmpty(request.ExternalId)) return null;
            var detail = new { request.ExternalId, IsPaymentSuccess = request.IsPaymentSuccess.GetValueOrDefault() };
            _ = await _repo.UpdateLevelChangeRequest(JsonConvert.SerializeObject(detail));
            var item = await _repo.GetLevelRequestById(request.ExternalId);
            return item;
        }

        public async Task<LevelRequestBo?> CompleteDiscountRequest(LevelRequestBo request)
        {
            if (string.IsNullOrEmpty(request.ExternalId)) return null;
            var detail = new { request.ExternalId, IsPaymentSuccess = request.IsPaymentSuccess.GetValueOrDefault() };
            var updated = await _repo.UpdateDiscountChangeRequest(JsonConvert.SerializeObject(detail));
            if (!updated.Key) return null;
            var item = await _repo.GetDiscountRequestById(request.ExternalId);
            return item;
        }

        public async Task<LevelRequestBo?> AddDiscountChangeRequest(LevelChangeRequest request)
        {
            _ = await _repo.AddDiscountChangeRequest(JsonConvert.SerializeObject(request));
            var item = await _repo.GetDiscountRequestById(request.ExternalId ?? string.Empty);
            return item;
        }


        public async Task<LevelRequestBo?> GetDiscountRequestById(string externalId)
        {
            var item = await _repo.GetDiscountRequestById(externalId);
            return item;
        }

        private static CustomerCreateOptions GenerateCreateOption(string email)
        {
            return new CustomerCreateOptions
            {
                Description = "Legal Lead Customer",
                Email = email
            };
        }

        [ExcludeFromCodeCoverage(Justification = "Implementation depends on 3rd party payment service.")]
        private async Task<string> CreateStripeCustomerAndGetIndex(string email)
        {
            try
            {
                var stripeCustomer = await GetCustomerService.CreateAsync(GenerateCreateOption(email));
                return stripeCustomer?.Id ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
