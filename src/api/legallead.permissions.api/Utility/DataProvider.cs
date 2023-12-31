using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.json.db.entity;

namespace legallead.permissions.api
{
    public class DataProvider : IDataProvider
    {
        private readonly IComponentRepository _componentDb;
        private readonly IPermissionMapRepository _permissionDb;
        private readonly IProfileMapRepository _profileDb;
        private readonly IUserPermissionRepository _userPermissionDb;
        private readonly IUserProfileRepository _userProfileDb;
        private readonly IUserTokenRepository _userTokenDb;
        private readonly IUserPermissionViewRepository _userPermissionVw;
        private readonly IUserProfileViewRepository _userProfileVw;
        private readonly IPermissionGroupRepository _permissionGroupDb;
        private readonly IUserRepository _userDb;
        private readonly IUserPermissionHistoryRepository _permissionHistoryDb;
        private readonly IUserProfileHistoryRepository _profileHistoryDb;

        internal DataProvider(
            IComponentRepository component,
            IPermissionMapRepository permissionDb,
            IProfileMapRepository profileDb,
            IUserPermissionRepository userPermissionDb,
            IUserProfileRepository userProfileDb,
            IUserTokenRepository userTokenDb,
            IUserPermissionViewRepository userPermissionVw,
            IUserProfileViewRepository userProfileVw,
            IPermissionGroupRepository permissionGroupDb,
            IUserRepository user,
            IUserPermissionHistoryRepository permissionHistoryDb,
            IUserProfileHistoryRepository profileHistoryDb)
        {
            _componentDb = component;
            _permissionDb = permissionDb;
            _profileDb = profileDb;
            _userPermissionDb = userPermissionDb;
            _userProfileDb = userProfileDb;
            _userTokenDb = userTokenDb;
            _userPermissionVw = userPermissionVw;
            _userProfileVw = userProfileVw;
            _permissionGroupDb = permissionGroupDb;
            _userDb = user;
            _permissionHistoryDb = permissionHistoryDb;
            _profileHistoryDb = profileHistoryDb;
        }

        internal IComponentRepository ComponentDb => _componentDb;
        internal IPermissionMapRepository PermissionDb => _permissionDb;
        internal IProfileMapRepository ProfileDb => _profileDb;
        internal IUserPermissionRepository UserPermissionDb => _userPermissionDb;
        internal IUserTokenRepository UserTokenDb => _userTokenDb;
        internal IPermissionGroupRepository PermissionGroupDb => _permissionGroupDb;
        internal IUserRepository UserDb => _userDb;
        public virtual IUserPermissionViewRepository UserPermissionVw => _userPermissionVw;
        public virtual IUserProfileRepository UserProfileDb => _userProfileDb;
        public virtual IUserProfileViewRepository UserProfileVw => _userProfileVw;
        public virtual IUserPermissionHistoryRepository PermissionHistoryDb => _permissionHistoryDb;
        public virtual IUserProfileHistoryRepository ProfileHistoryDb => _profileHistoryDb;

        public virtual async Task<bool> InitializeProfile(User user)
        {
            var profiles = await ProfileDb.GetAll();
            if (!profiles.Any()) { return true; }
            var current = profiles.Select(p => new UserProfile
            {
                Id = Guid.NewGuid().ToString("D"),
                ProfileMapId = p.Id ?? string.Empty,
                UserId = user.Id ?? string.Empty,
                KeyValue = string.Empty
            }).ToList();
            var existing = await UserProfileDb.GetAll(user);
            var additions = current.FindAll(x => !existing.Any(a => a.ProfileMapId == x.ProfileMapId));
            if (!additions.Any()) { return true; }
            additions.ForEach(async a => await UserProfileDb.Create(a));
            return true;
        }

        public virtual async Task<bool> InitializePermission(User user)
        {
            var permissions = await PermissionDb.GetAll();
            if (!permissions.Any()) { return true; }
            var current = permissions.Select(p => new UserPermission
            {
                Id = Guid.NewGuid().ToString("D"),
                PermissionMapId = p.Id ?? string.Empty,
                UserId = user.Id ?? string.Empty,
                KeyValue = string.Empty
            }).ToList();
            var existing = await UserPermissionDb.GetAll(user);
            var additions = current.FindAll(x => !existing.Any(a => a.PermissionMapId == x.PermissionMapId));
            if (!additions.Any()) { return true; }
            additions.ForEach(async a => await UserPermissionDb.Create(a));
            return true;
        }

        public virtual async Task<KeyValuePair<bool, string>> SetPermissionGroup(User user, string groupName)
        {
            try
            {
                var groups = ((await PermissionGroupDb.GetAll()) ?? Array.Empty<PermissionGroup>()).ToList();
                var permissions = ((await UserPermissionVw.GetAll(user)) ?? Array.Empty<UserPermissionView>()).ToList();
                if (!groups.Any()) return new KeyValuePair<bool, string>(false, "No groups defined in repository.");
                if (!permissions.Any()) return new KeyValuePair<bool, string>(false, "No permissions defined for user.");
                var group = groups.Find(g => g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase) && g.IsActive.GetValueOrDefault());
                if (group == null) return new KeyValuePair<bool, string>(false, $"Group with name '{groupName} not defined.");
                var settings = new Dictionary<string, string>()
                {
                    { "Account.IsPrimary", "True" },
                    { "Account.Permission.Level", group.Name ?? string.Empty },
                    { "Setting.MaxRecords.Per.Year", group.PerYear.GetValueOrDefault().ToString() ?? string.Empty },
                    { "Setting.MaxRecords.Per.Month", group.PerMonth.GetValueOrDefault().ToString()},
                    { "Setting.MaxRecords.Per.Request", group.PerRequest.GetValueOrDefault().ToString() },
                    { "Setting.Pricing.Name", group.Name ?? string.Empty }
                };
                var changes = ApplyChanges(user, settings, permissions);
                if (!changes.Key) return changes;
                var answer = new KeyValuePair<bool, string>(true, $"Group settings for '{groupName}' applied to user: {user.UserName}.");
                var groupPrice = groups.Find(g => g.Name.Equals($"{groupName}.Pricing", StringComparison.OrdinalIgnoreCase) && g.IsActive.GetValueOrDefault());
                if (groupPrice == null) return answer;
                settings = new Dictionary<string, string>()
                {
                    { "Setting.Pricing.Per.Year", groupPrice.PerYear.GetValueOrDefault().ToString() ?? string.Empty },
                    { "Setting.Pricing.Per.Month", groupPrice.PerMonth.GetValueOrDefault().ToString()},
                    { "Setting.Pricing.Per.Request", groupPrice.PerRequest.GetValueOrDefault().ToString() },
                };
                changes = ApplyChanges(user, settings, permissions);
                if (!changes.Key) return changes;
                return answer;
            }
            catch (Exception)
            {
                return new KeyValuePair<bool, string>(false, $"Unexpected error occurred appling user settings.");
            }
        }

        public virtual async Task<KeyValuePair<bool, string>> AddStateSubscriptions(User user, string stateCode)
        {
            try
            {
                const string search = ".State.Subscriptions";
                var groups = ((await PermissionGroupDb.GetAll()) ?? Array.Empty<PermissionGroup>()).ToList();
                var permissions = ((await UserPermissionVw.GetAll(user)) ?? Array.Empty<UserPermissionView>()).ToList();
                if (!groups.Any()) return new KeyValuePair<bool, string>(false, "No groups defined in repository.");
                if (!permissions.Any()) return new KeyValuePair<bool, string>(false, "No permissions defined for user.");
                var group = groups.Find(g => g.Name.Contains(search, StringComparison.OrdinalIgnoreCase) && g.IsActive.GetValueOrDefault());
                if (group == null) return new KeyValuePair<bool, string>(false, $"Group with name '{stateCode} not defined.");
                var settings = new Dictionary<string, string>()
                {
                    { "Setting.State.Subscriptions", stateCode },
                    { "Setting.State.Subscriptions.Active", "True" }
                };
                var changes = ApplySubscriptions(user, settings, permissions);
                if (!changes.Key) return changes;
                var discounts = ApplyDiscounts(user, permissions, groups, "State");
                if (!discounts.Key) return discounts;
                return new KeyValuePair<bool, string>(true, $"Group settings for '{stateCode}' applied to user: {user.UserName}.");
            }
            catch (Exception)
            {
                return new KeyValuePair<bool, string>(false, $"Unexpected error occurred appling user settings.");
            }
        }

        public virtual async Task<KeyValuePair<bool, string>> RemoveStateSubscriptions(User user, string stateCode)
        {
            try
            {
                const string search = ".State.Subscriptions";
                var groups = ((await PermissionGroupDb.GetAll()) ?? Array.Empty<PermissionGroup>()).ToList();
                var permissions = ((await UserPermissionVw.GetAll(user)) ?? Array.Empty<UserPermissionView>()).ToList();
                if (!groups.Any()) return new KeyValuePair<bool, string>(false, "No groups defined in repository.");
                if (!permissions.Any()) return new KeyValuePair<bool, string>(false, "No permissions defined for user.");
                var group = groups.Find(g => g.Name.Contains(search, StringComparison.OrdinalIgnoreCase) && g.IsActive.GetValueOrDefault());
                if (group == null) return new KeyValuePair<bool, string>(false, $"Group with name '{stateCode} not defined.");
                var settings = new Dictionary<string, string>()
                {
                    { "Setting.State.Subscriptions", stateCode },
                    { "Setting.State.Subscriptions.Active", "False" }
                };
                var changes = RemoveSubscriptions(user, settings, permissions);
                if (!changes.Key) return changes;
                if (IsDiscountRemovalNeeded(changes.Value))
                {
                    var removal = RemoveDiscounts(user, permissions, "State");
                    if (!removal.Key) return removal;
                }
                return new KeyValuePair<bool, string>(true, $"Group settings for '{stateCode}' applied to user: {user.UserName}.");
            }
            catch (Exception)
            {
                return new KeyValuePair<bool, string>(false, $"Unexpected error occurred appling user settings.");
            }
        }

        public virtual async Task<KeyValuePair<bool, string>> AddCountySubscriptions(User user, UsStateCounty countyCode)
        {
            try
            {
                const string search = ".County.";
                var groups = ((await PermissionGroupDb.GetAll()) ?? Array.Empty<PermissionGroup>()).ToList();
                var permissions = ((await UserPermissionVw.GetAll(user)) ?? Array.Empty<UserPermissionView>()).ToList();
                if (!groups.Any()) return new KeyValuePair<bool, string>(false, "No groups defined in repository.");
                if (!permissions.Any()) return new KeyValuePair<bool, string>(false, "No permissions defined for user.");
                var group = groups.Find(g => g.Name.Contains(search, StringComparison.OrdinalIgnoreCase) && g.IsActive.GetValueOrDefault());
                if (group == null) return new KeyValuePair<bool, string>(false, $"Group with name '{countyCode.Name}, {countyCode.StateCode} not defined.");
                var settings = new Dictionary<string, string>()
                {
                    { "Setting.State.County.Subscriptions", countyCode.Index.ToString() },
                    { "Setting.State.County.Subscriptions.Active", "True" }
                };
                var changes = ApplySubscriptions(user, settings, permissions);
                if (!changes.Key) return changes;
                var discounts = ApplyDiscounts(user, permissions, groups, "County");
                if (!discounts.Key) return discounts;
                return new KeyValuePair<bool, string>(true, $"Group settings for '{countyCode.Name}' applied to user: {user.UserName}.");
            }
            catch (Exception)
            {
                return new KeyValuePair<bool, string>(false, $"Unexpected error occurred appling user settings.");
            }
        }

        public virtual async Task<KeyValuePair<bool, string>> RemoveCountySubscriptions(User user, UsStateCounty countyCode)
        {
            try
            {
                const string search = ".County.";
                var groups = ((await PermissionGroupDb.GetAll()) ?? Array.Empty<PermissionGroup>()).ToList();
                var permissions = ((await UserPermissionVw.GetAll(user)) ?? Array.Empty<UserPermissionView>()).ToList();
                if (!groups.Any()) return new KeyValuePair<bool, string>(false, "No groups defined in repository.");
                if (!permissions.Any()) return new KeyValuePair<bool, string>(false, "No permissions defined for user.");
                var group = groups.Find(g => g.Name.Contains(search, StringComparison.OrdinalIgnoreCase) && g.IsActive.GetValueOrDefault());
                if (group == null) return new KeyValuePair<bool, string>(false, $"Group with name '{countyCode.Name}, {countyCode.StateCode} not defined.");
                var settings = new Dictionary<string, string>()
                {
                    { "Setting.State.County.Subscriptions", countyCode.Index.ToString() },
                    { "Setting.State.County.Subscriptions.Active", "True" }
                };
                var changes = RemoveSubscriptions(user, settings, permissions);
                if (!changes.Key) return changes;
                if (IsDiscountRemovalNeeded(changes.Value))
                {
                    var removal = RemoveDiscounts(user, permissions, "County");
                    if (!removal.Key) return removal;
                }
                return new KeyValuePair<bool, string>(true, $"Group settings for '{countyCode.Name}' applied to user: {user.UserName}.");
            }
            catch (Exception)
            {
                return new KeyValuePair<bool, string>(false, $"Unexpected error occurred appling user settings.");
            }
        }

        private KeyValuePair<bool, string> ApplySubscriptions(User user, Dictionary<string, string> settings, List<UserPermissionView> permissions)
        {
            var changes = new List<UserPermission>();
            foreach (var item in settings.Keys)
            {
                var permission = permissions.Find(p => p.KeyName.Equals(item, StringComparison.OrdinalIgnoreCase));
                if (permission == null) return new KeyValuePair<bool, string>(false, $"Expected key: '{item}' not found in user settings");
                var currentSetting = permission.KeyValue;
                var desiredSetting = settings[item];
                var isValue = !item.Contains("Active");
                if (isValue && currentSetting.Contains(desiredSetting, StringComparison.OrdinalIgnoreCase)) continue;
                if (isValue && !string.IsNullOrEmpty(currentSetting)) { desiredSetting = $"{currentSetting}, {desiredSetting}"; }
                changes.Add(new UserPermission
                {
                    Id = permission.Id,
                    UserId = user.Id ?? Guid.Empty.ToString(),
                    PermissionMapId = permission.PermissionMapId,
                    KeyValue = SortItems(desiredSetting)
                });
            }
            changes.ForEach(async c =>
            {
                await UserPermissionDb.Update(c);
            });
            return new KeyValuePair<bool, string>(true, $"Keys updated successfully");
        }

        private KeyValuePair<bool, string> RemoveSubscriptions(User user, Dictionary<string, string> settings, List<UserPermissionView> permissions)
        {
            var changes = new List<UserPermission>();
            string currentSubscriptions = "";
            foreach (var item in settings.Keys)
            {
                var permission = permissions.Find(p => p.KeyName.Equals(item, StringComparison.OrdinalIgnoreCase));
                if (permission == null) return new KeyValuePair<bool, string>(false, $"Expected key: '{item}' not found in user settings");
                var currentSetting = permission.KeyValue;
                var desiredSetting = settings[item];
                var isValue = !item.Contains("Active");
                if (isValue)
                {
                    currentSubscriptions = RemoveKey(currentSetting, desiredSetting);
                    desiredSetting = currentSubscriptions;
                }
                else
                {
                    desiredSetting = string.IsNullOrEmpty(currentSubscriptions) ? "False" : "True";
                }
                changes.Add(new UserPermission
                {
                    Id = permission.Id,
                    UserId = user.Id ?? Guid.Empty.ToString(),
                    PermissionMapId = permission.PermissionMapId,
                    KeyValue = SortItems(desiredSetting)
                });
            }
            changes.ForEach(async c =>
            {
                await UserPermissionDb.Update(c);
            });
            return new KeyValuePair<bool, string>(true, $"Keys updated successfully | Current Subscriptions := {currentSubscriptions}");
        }

        private KeyValuePair<bool, string> ApplyDiscounts(User user, List<UserPermissionView> permissions, List<PermissionGroup> groups, string discountCode)
        {
            var discountNames = "State,County".Split(',').ToList();
            if (!discountNames.Exists(x => x.Equals(discountCode, StringComparison.OrdinalIgnoreCase)))
            {
                return new KeyValuePair<bool, string>(false, $"Invalid discount type key: '{discountCode}'.");
            }
            var search = $"{discountCode}.Discount.Pricing";
            var group = groups.Find(g => g.Name.Contains(search, StringComparison.OrdinalIgnoreCase) && g.IsActive.GetValueOrDefault());
            if (group == null) return new KeyValuePair<bool, string>(false, $"Group with name '{search} not defined.");
            var settingName = discountCode.Equals(discountNames[0], StringComparison.OrdinalIgnoreCase) ? "User.State.Discount" : "User.State.County.Discount";
            var settings = new Dictionary<string, string>() {
                { settingName, group.PerRequest.GetValueOrDefault().ToString() }
            };
            return ApplyChanges(user, settings, permissions);
        }

        private KeyValuePair<bool, string> RemoveDiscounts(User user, List<UserPermissionView> permissions, string discountCode)
        {
            var discountNames = "State,County".Split(',').ToList();
            if (!discountNames.Exists(x => x.Equals(discountCode, StringComparison.OrdinalIgnoreCase)))
            {
                return new KeyValuePair<bool, string>(false, $"Invalid discount type key: '{discountCode}'.");
            }
            var settingName = discountCode.Equals(discountNames[0], StringComparison.OrdinalIgnoreCase) ? "User.State.Discount" : "User.State.County.Discount";
            var settings = new Dictionary<string, string>() { { settingName, string.Empty } };
            return ApplyChanges(user, settings, permissions);
        }

        private KeyValuePair<bool, string> ApplyChanges(User user, Dictionary<string, string> settings, List<UserPermissionView> permissions)
        {
            var changes = new List<UserPermission>();
            foreach (var item in settings.Keys)
            {
                var permission = permissions.Find(p => p.KeyName.Equals(item, StringComparison.OrdinalIgnoreCase));
                if (permission == null) return new KeyValuePair<bool, string>(false, $"Expected key: '{item}' not found in user settings");
                changes.Add(new UserPermission
                {
                    Id = permission.Id,
                    UserId = user.Id ?? Guid.Empty.ToString(),
                    PermissionMapId = permission.PermissionMapId,
                    KeyValue = settings[item]
                });
            }
            changes.ForEach(async c =>
            {
                await UserPermissionDb.Update(c);
            });
            return new KeyValuePair<bool, string>(true, $"Keys updated successfully");
        }

        private static string SortItems(string source)
        {
            const char comma = ',';
            if (string.IsNullOrEmpty(source) || !source.Contains(',')) return source;
            var items = source.Split(comma).ToList();
            items.Sort();
            return string.Join(comma, items);
        }

        private static string RemoveKey(string current, string key)
        {
            const char comma = ',';
            if (string.IsNullOrEmpty(current)) return current;
            if (string.IsNullOrEmpty(key)) return current;
            if (!current.Contains(comma)) return string.Empty;
            var items = current.Split(comma).ToList();
            items.Remove(key);
            items.Sort();
            return string.Join(comma, items);
        }

        private static bool IsDiscountRemovalNeeded(string? removalText)
        {
            const char pipe = '|';
            const char eq = '=';
            if (string.IsNullOrWhiteSpace(removalText)) return false;
            if (!removalText.Contains(pipe) || !removalText.Contains(eq)) return false;
            var command = removalText.Split(pipe)[^1];
            if (!command.Contains(eq)) return false;
            var setting = command.Split(eq)[^1].Trim();
            return string.IsNullOrEmpty(setting);
        }
    }
}