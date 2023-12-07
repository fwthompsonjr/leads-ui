using legallead.jdbc.entities;
using legallead.json.db;
using legallead.json.db.entity;

namespace legallead.permissions.api.Utility
{
    public class SubscriptionInfrastructure : ISubscriptionInfrastructure
    {
        private readonly DataProvider _db;
        public SubscriptionInfrastructure(DataProvider db)
        {
            _db = db;
        }
        public async Task<KeyValuePair<bool, string>> AddCountySubscriptions(User user, UsStateCounty countyCode)
        {
            return await _db.AddCountySubscriptions(user, countyCode);
        }

        public async Task<KeyValuePair<bool, string>> AddStateSubscriptions(User user, string stateCode)
        {
            return await _db.AddStateSubscriptions(user, stateCode);
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
            return await _db.RemoveCountySubscriptions(user, countyCode);
        }

        public async Task<KeyValuePair<bool, string>> RemoveStateSubscriptions(User user, string stateCode)
        {
            return await _db.RemoveStateSubscriptions(user, stateCode);
        }

        public async Task<KeyValuePair<bool, string>> SetPermissionGroup(User user, string groupName)
        {
            return await _db.SetPermissionGroup(user, groupName);
        }
    }
}
