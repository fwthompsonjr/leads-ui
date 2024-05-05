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

        public async Task<KeyValuePair<bool, string>> AddCountySubscriptions(User user, UsStateCounty countyCode)
        {
            var response = await _db.AddCountySubscriptions(user, countyCode);
            if (response.Key)
                await _db.PermissionHistoryDb.CreateSnapshot(user, jdbc.PermissionChangeTypes.SubscriptionCountyChanged);
            return response;
        }

        public async Task<KeyValuePair<bool, string>> AddStateSubscriptions(User user, string stateCode)
        {
            var response = await _db.AddStateSubscriptions(user, stateCode);
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

        public async Task<User?> GetUser(HttpRequest request)
        {
            return await request.GetUser(_db);
        }

        public async Task<bool> IsAdminUser(HttpRequest request)
        {
            return await request.IsAdminUser(_db);
        }

        public async Task<KeyValuePair<bool, string>> RemoveCountySubscriptions(User user, UsStateCounty countyCode)
        {
            var response = await _db.RemoveCountySubscriptions(user, countyCode);
            if (response.Key)
                await _db.PermissionHistoryDb.CreateSnapshot(user, jdbc.PermissionChangeTypes.SubscriptionCountyChanged);
            return response;
        }

        public async Task<KeyValuePair<bool, string>> RemoveStateSubscriptions(User user, string stateCode)
        {
            var response = await _db.RemoveStateSubscriptions(user, stateCode);
            if (response.Key)
                await _db.PermissionHistoryDb.CreateSnapshot(user, jdbc.PermissionChangeTypes.SubscriptionStateChanged);
            return response;
        }

        public async Task<KeyValuePair<bool, string>> SetPermissionGroup(User user, string groupName)
        {
            var response = await _db.SetPermissionGroup(user, groupName);
            if (response.Key)
                await _db.PermissionHistoryDb.CreateSnapshot(user, jdbc.PermissionChangeTypes.PermissionLevelChanged);
            return response;
        }
    }
}