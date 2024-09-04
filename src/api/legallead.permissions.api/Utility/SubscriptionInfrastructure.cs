using legallead.json.db;
using legallead.json.db.entity;
using legallead.permissions.api.Models;

namespace legallead.permissions.api.Utility
{
    public partial class SubscriptionInfrastructure : ISubscriptionInfrastructure
    {
        protected readonly IDataProvider _db;
        protected readonly ICustomerInfrastructure? _customer;
        protected readonly PaymentStripeOption? _payment;

        public SubscriptionInfrastructure(IDataProvider db, ICustomerInfrastructure customer, PaymentStripeOption payment)
        {
            _db = db;
            _customer = customer;
            _payment = payment;
        }
        internal SubscriptionInfrastructure(IDataProvider db)
        {
            _db = db;
        }

        public async Task<KeyValuePair<bool, string>> AddCountySubscriptionsAsync(User user, UsStateCounty countyCode)
        {
            var response = await _db.AddCountySubscriptionsAsync(user, countyCode);
            if (response.Key)
                await _db.PermissionHistoryDb.CreateSnapshot(user, jdbc.PermissionChangeTypes.SubscriptionCountyChanged);
            return response;
        }

        public async Task<KeyValuePair<bool, string>> AddStateSubscriptionsAsync(User user, string stateCode)
        {
            var response = await _db.AddStateSubscriptionsAsync(user, stateCode);
            if (response.Key)
                await _db.PermissionHistoryDb.CreateSnapshot(user, jdbc.PermissionChangeTypes.SubscriptionStateChanged);
            return response;
        }

        public List<UsStateCounty>? FindAllCounties(string? county)
        {
            return UsStateCountyList.FindAll(county);
        }

        public UsState? FindState(string? state)
        {
            return UsStatesList.Find(state);
        }

        public async Task<User?> GetUserAsync(HttpRequest request)
        {
            return await request.GetUserAsync(_db);
        }

        public async Task<bool> IsAdminUserAsync(HttpRequest request)
        {
            return await request.IsAdminUserAsync(_db);
        }

        public async Task<KeyValuePair<bool, string>> RemoveCountySubscriptionsAsync(User user, UsStateCounty countyCode)
        {
            var response = await _db.RemoveCountySubscriptionsAsync(user, countyCode);
            if (response.Key)
                await _db.PermissionHistoryDb.CreateSnapshot(user, jdbc.PermissionChangeTypes.SubscriptionCountyChanged);
            return response;
        }

        public async Task<KeyValuePair<bool, string>> RemoveStateSubscriptionsAsync(User user, string stateCode)
        {
            var response = await _db.RemoveStateSubscriptionsAsync(user, stateCode);
            if (response.Key)
                await _db.PermissionHistoryDb.CreateSnapshot(user, jdbc.PermissionChangeTypes.SubscriptionStateChanged);
            return response;
        }

        public async Task<KeyValuePair<bool, string>> SetPermissionGroupAsync(User user, string groupName)
        {
            var response = await _db.SetPermissionGroupAsync(user, groupName);
            if (response.Key)
                await _db.PermissionHistoryDb.CreateSnapshot(user, jdbc.PermissionChangeTypes.PermissionLevelChanged);
            return response;
        }
    }
}