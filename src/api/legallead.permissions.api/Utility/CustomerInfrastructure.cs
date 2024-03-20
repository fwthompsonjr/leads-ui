using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.json.db.entity;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Model;
using legallead.permissions.api.Models;
using Newtonsoft.Json;
using Stripe;
using System.Collections.Generic;

namespace legallead.permissions.api.Utility
{
    public class CustomerInfrastructure : ICustomerInfrastructure
    {
        private readonly CustomerService _customerService;
        private readonly string _paymentEnvironment;
        private readonly ICustomerRepository _repo;
        private readonly IUserRepository _db;
        private ISubscriptionInfrastructure? _subscriptiondb;
        public CustomerInfrastructure(
            StripeKeyEntity stripeKey,
            IUserRepository db, 
            ICustomerRepository repository)
        {
            _customerService = new();
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
            _ = await _repo.UpdateDiscountChangeRequest(JsonConvert.SerializeObject(detail));
            var changes = GetRequest(request);
            if (!detail.IsPaymentSuccess || changes == null) { return null; }
            var user = await _db.GetById(request.UserId ?? string.Empty);
            if (user == null) { return null; }

            var statelist = ModelMapper.Mapper.Map<List<KeyValuePair<bool, UsState>>>(changes);
            var stateResponse = await ProcessStateDiscounts(user, statelist);
            if (stateResponse.Exists(a => !a)) return null;

            var countylist = ModelMapper.Mapper.Map<List<KeyValuePair<bool, UsStateCounty>>>(changes);
            var countyResponse = await ProcessCountyDiscounts(user, countylist);
            if (countyResponse.Exists(a => !a)) return null;

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



        protected virtual async Task<bool> AddCountySubscriptions(User user, CountySubscriptionRequest request)
        {
            if (_subscriptiondb == null) return false;
            var stateId = json.db.UsStatesList.Find(request.State);
            if (stateId == null)
            {
                return false;
            }
            var countyList = json.db.UsStateCountyList.FindAll(request.County);
            var countyId = countyList?.Find(l => (l.StateCode ?? "").Equals(stateId.ShortName));
            if (countyId == null)
            {
                return false;
            }
            var response = await _subscriptiondb.AddCountySubscriptions(user, countyId);

            return response.Key;
        }
        protected virtual async Task<bool> AddStateSubscriptions(User user, StateSubscriptionRequest request)
        {
            if (_subscriptiondb == null) return false;
            var stateId = json.db.UsStatesList.Find(request.Name);
            if (stateId == null || string.IsNullOrEmpty(stateId.ShortName))
            {
                return false;
            }
            var response = await _subscriptiondb.AddStateSubscriptions(user, stateId.ShortName);
            return response.Key;
        }

        protected virtual async Task<bool> RemoveStateSubscriptions(User user, StateSubscriptionRequest request)
        {
            if (_subscriptiondb == null) return false;
            var stateId = json.db.UsStatesList.Find(request.Name);
            if (stateId == null || string.IsNullOrEmpty(stateId.ShortName))
            {
                return false;
            }
            var response = await _subscriptiondb.RemoveStateSubscriptions(user, stateId.ShortName);
            return response.Key;
        }

        protected virtual async Task<bool> RemoveCountySubscriptions(User user, CountySubscriptionRequest request)
        {
            if (_subscriptiondb == null) return false;
            var stateId = json.db.UsStatesList.Find(request.State);
            if (stateId == null)
            {
                return false;
            }
            var countyList = json.db.UsStateCountyList.FindAll(request.County);
            var countyId = countyList?.Find(l => (l.StateCode ?? "").Equals(stateId.ShortName));
            if (countyId == null)
            {
                return false;
            }
            var response = await _subscriptiondb.RemoveCountySubscriptions(user, countyId);
            return response.Key;
        }

        private static CustomerCreateOptions GenerateCreateOption(string email)
        {
            return new CustomerCreateOptions
            {
                Description = "Legal Lead Customer",
                Email = email
            };
        }

        private static ChangeDiscountRequest? GetRequest(LevelRequestBo bo)
        {
            if (string.IsNullOrWhiteSpace(bo.LevelName)) return null;

            var source = JsonConvert.DeserializeObject<DiscountChangeParent>(bo.LevelName);
            if (source == null) return null;
            return ModelMapper.Mapper.Map<ChangeDiscountRequest>(source);
        }

        private async Task<List<bool>> ProcessStateDiscounts(User user, List<KeyValuePair<bool, UsState>> statelist)
        {
            List<bool> list = new();
            await Task.Run(() =>
            {
                statelist.ForEach(async st =>
                {
                    var failed = list.Exists(a => !a);
                    if (!failed)
                    {
                        bool? stateResult;
                        var stateRequest = new StateSubscriptionRequest { Name = st.Value.Name };
                        if (st.Key)
                        {
                            stateResult = await AddStateSubscriptions(user, stateRequest);
                        }
                        else
                        {
                            stateResult = await RemoveStateSubscriptions(user, stateRequest);
                        }
                        list.Add(stateResult.GetValueOrDefault());
                    }
                });
            });
            return list;
        }

        private async Task<List<bool>> ProcessCountyDiscounts(User user, List<KeyValuePair<bool, UsStateCounty>> countylist)
        {
            List<bool> list = new();
            await Task.Run(() =>
            {
                countylist.ForEach(async c =>
                {
                    var failed = list.Exists(a => !a);
                    if (!failed)
                    {
                        bool? countyResult;
                        var county = c.Value;
                        var countyRequest = new CountySubscriptionRequest
                        {
                            County = county.Name,
                            State = county.StateCode
                        };
                        if (c.Key)
                        {
                            countyResult = await AddCountySubscriptions(user, countyRequest);
                        }
                        else
                        {
                            countyResult = await RemoveCountySubscriptions(user, countyRequest);
                        }
                        list.Add(countyResult.GetValueOrDefault());
                    }
                });
            });
            return list;
        }
    }
}
