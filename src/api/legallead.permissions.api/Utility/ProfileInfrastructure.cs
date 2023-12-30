using AutoMapper;
using legallead.jdbc.entities;
using legallead.permissions.api.Model;
using System.Linq;

namespace legallead.permissions.api.Utility
{
    public class ProfileInfrastructure : SubscriptionInfrastructure, IProfileInfrastructure
    {
        private readonly IMapper mapper;

        public ProfileInfrastructure(IDataProvider db) : base(db)
        {
            mapper = ModelMapper.Mapper;
        }

        public async Task<GetContactResponse[]> GetContactDetail(User? user, string responseType)
        {
            var fallback = new GetContactResponse[] {
                new() { ResponseType = "Error", Message = "Unable to retrieve user detail" }
                };
            if (user == null) { return fallback; }
            try
            {
                var current = await _db.UserProfileVw.GetAll(user);
                var response = mapper.Map<GetContactResponse[]>(current.ToArray()).ToList();
                if (string.IsNullOrEmpty(responseType)) { return response.ToArray(); }
                return response.FindAll(x => x.ResponseType == responseType).ToArray();
            }
            catch (Exception ex)
            {
                fallback[0].Message = ex.Message;
                return fallback;
            }
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
                r.KeyName ??= string.Empty;
                var found = current.FirstOrDefault(c => (c.KeyName ?? "").Equals(r.KeyName));
                found ??= current.FirstOrDefault(c => (c.KeyName ?? "").StartsWith(r.KeyName));
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