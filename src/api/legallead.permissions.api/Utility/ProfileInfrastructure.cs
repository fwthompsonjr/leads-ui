using AutoMapper;
using legallead.jdbc.entities;
using legallead.permissions.api.Model;

namespace legallead.permissions.api.Utility
{
    public class ProfileInfrastructure : SubscriptionInfrastructure, IProfileInfrastructure
    {
        private readonly IMapper mapper;
        public ProfileInfrastructure(IDataProvider db) : base(db)
        {
            mapper = ModelMapper.Mapper;
        }

        public async Task<KeyValuePair<bool, string>> ChangeContactAddress(User? user, ChangeContactAddressRequest[] request)
        {
            var updated = await ChangeContact(_db, mapper, user, request);
            if (!updated.Key || user == null) return new KeyValuePair<bool, string>(false, "Error occurred updating user profile.");
            await _db.ProfileHistoryDb.CreateSnapshot(user, jdbc.ProfileChangeTypes.UserAddressDetailChanged);
            return new KeyValuePair<bool, string>(true, "Contact Address details updated successfully.");
        }

        public async Task<KeyValuePair<bool, string>> ChangeContactEmail(User? user, ChangeContactEmailRequest[] request)
        {
            var updated = await ChangeContact(_db, mapper, user, request);
            if (!updated.Key || user == null) return new KeyValuePair<bool, string>(false, "Error occurred updating user profile.");
            await _db.ProfileHistoryDb.CreateSnapshot(user, jdbc.ProfileChangeTypes.UserEmailAddressChanged);
            return new KeyValuePair<bool, string>(true, "Contact Email details updated successfully.");
        }

        public async Task<KeyValuePair<bool, string>> ChangeContactName(User? user, ChangeContactNameRequest[] request)
        {
            var updated = await ChangeContact(_db, mapper, user, request);
            if (!updated.Key || user == null) return new KeyValuePair<bool, string>(false, "Error occurred updating user profile.");
            await _db.ProfileHistoryDb.CreateSnapshot(user, jdbc.ProfileChangeTypes.UserEmailAddressChanged);
            return new KeyValuePair<bool, string>(true, "Contact Name details updated successfully.");
        }

        public async Task<KeyValuePair<bool, string>> ChangeContactPhone(User? user, ChangeContactPhoneRequest[] request)
        {
            var updated = await ChangeContact(_db, mapper, user, request);
            if (!updated.Key || user == null) return new KeyValuePair<bool, string>(false, "Error occurred updating user profile.");
            await _db.ProfileHistoryDb.CreateSnapshot(user, jdbc.ProfileChangeTypes.UserEmailAddressChanged);
            return new KeyValuePair<bool, string>(true, "Contact Phone details updated successfully.");
        }

        private static async Task<KeyValuePair<bool, string>> ChangeContact(IDataProvider db, IMapper mapper, User? user, object request)
        {
            if (user == null) return new KeyValuePair<bool, string>(false, "Invalid user context");
            var current = await db.UserProfileVw.GetAll(user);
            var requests = mapper.Map<UserProfileView[]>(request).ToList();
            var updates = new List<UserProfile>();
            requests.ForEach(r =>
            {
                var found = current.First(c => (c.KeyName ?? "").Equals(r.KeyName));
                if (found != null)
                {
                    found.KeyValue = r.KeyValue;
                    updates.Add(mapper.Map<UserProfile>(found));
                }
            });
            var updated = ApplyProfileChanges(db, updates);
            if (!updated) return new KeyValuePair<bool, string>(false, "Error occurred updating user profile.");
            return new KeyValuePair<bool, string>(true, "update completed.");
        }

        private static bool ApplyProfileChanges(IDataProvider db, List<UserProfile> source)
        {
            try
            {
                source.ForEach(async c =>
                    {
                        await db.UserProfileDb.Update(c);
                    });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
