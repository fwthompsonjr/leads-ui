using legallead.jdbc.interfaces;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Models;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;
using Stripe;
using System.Text.Json;

namespace legallead.permissions.api.Utility
{
    public class CustomerInfrastructure(
        StripeKeyEntity stripeKey,
        IUserRepository db,
        ICustomerRepository repository) : ICustomerInfrastructure
    {
        private readonly string _paymentEnvironment = stripeKey.ActiveName;
        private readonly ICustomerRepository _repo = repository;
        private readonly IUserRepository _db = db;
        private ISubscriptionInfrastructure? _subscriptiondb;

        internal CustomerInfrastructure(
            StripeKeyEntity stripeKey,
            IUserRepository db,
            ICustomerRepository repository,
            ISubscriptionInfrastructure subscription) :
            this(stripeKey, db, repository)
        {
            _subscriptiondb = subscription;
        }

        internal CustomerService GetCustomerService { get; set; } = new();

        internal void SubscriptionInfrastructure(ISubscriptionInfrastructure subscription)
        {
            if (_subscriptiondb != null) { return; }
            _subscriptiondb = subscription;
        }

        public async Task<PaymentCustomerBo?> CreateCustomerAsync(string userId, string accountId)
        {
            var user = await _db.GetById(userId);
            if (user == null) { return null; }
            var response = await _repo.AddCustomer(new() { AccountType = _paymentEnvironment, CustomerId = accountId, UserId = userId });
            if (!response.Key) return null;
            var customer = await _repo.GetCustomer(new() { AccountType = _paymentEnvironment, UserId = userId });
            return customer;
        }


        public async Task<PaymentCustomerBo?> GetOrCreateCustomerAsync(string userId)
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
            var stripeCustomerId = await CreateStripeCustomerAndGetIndexAsync(user.Email);
            var creation = await CreateCustomerAsync(user.Id, stripeCustomerId);
            return creation;
        }

        public async Task<PaymentCustomerBo?> GetCustomerAsync(string userId)
        {
            var customer = await GetOrCreateCustomerAsync(userId);
            return customer;
        }
        public async Task<List<UnMappedCustomerBo>?> GetUnMappedCustomersAsync()
        {
            var response = await _repo.GetUnMappedCustomers(new() { AccountType = _paymentEnvironment });
            return response;
        }

        public async Task<bool> MapCustomersAsync()
        {
            try
            {
                var jwt = ThreadHelper.JoinableTaskFactory;
                var customers = await GetUnMappedCustomersAsync();
                if (customers == null || customers.Count == 0) { return true; }
                customers.ForEach(customer =>
                    {
                        if (!string.IsNullOrEmpty(customer.Id))
                        {
                            jwt.Run(async delegate
                            {
                                _ = await GetOrCreateCustomerAsync(customer.Id);
                            });
                        }
                    });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<LevelRequestBo?> AddLevelChangeRequestAsync(LevelChangeRequest request)
        {
            _ = await _repo.AddLevelChangeRequest(JsonConvert.SerializeObject(request));
            var item = await _repo.GetLevelRequestById(request.ExternalId ?? string.Empty);
            return item;
        }

        public async Task<LevelRequestBo?> GetLevelRequestByIdAsync(string externalId)
        {
            var item = await _repo.GetLevelRequestById(externalId);
            return item;
        }

        public async Task<LevelRequestBo?> CompleteLevelRequestAsync(LevelRequestBo request)
        {
            if (string.IsNullOrEmpty(request.ExternalId)) return null;
            var detail = new { request.ExternalId, IsPaymentSuccess = request.IsPaymentSuccess.GetValueOrDefault() };
            _ = await _repo.UpdateLevelChangeRequest(JsonConvert.SerializeObject(detail));
            var item = await _repo.GetLevelRequestById(request.ExternalId);
            return item;
        }

        public async Task<LevelRequestBo?> CompleteDiscountRequestAsync(LevelRequestBo request)
        {
            if (string.IsNullOrEmpty(request.ExternalId)) return null;
            var detail = new { request.ExternalId, IsPaymentSuccess = request.IsPaymentSuccess.GetValueOrDefault() };
            var updated = await _repo.UpdateDiscountChangeRequest(JsonConvert.SerializeObject(detail));
            if (!updated.Key) return null;
            var item = await _repo.GetDiscountRequestById(request.ExternalId);
            return item;
        }

        public async Task<LevelRequestBo?> AddDiscountChangeRequestAsync(LevelChangeRequest request)
        {
            _ = await _repo.AddDiscountChangeRequest(JsonConvert.SerializeObject(request));
            var item = await _repo.GetDiscountRequestById(request.ExternalId ?? string.Empty);
            return item;
        }


        public async Task<LevelRequestBo?> GetDiscountRequestByIdAsync(string externalId)
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
        private async Task<string> CreateStripeCustomerAndGetIndexAsync(string email)
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
