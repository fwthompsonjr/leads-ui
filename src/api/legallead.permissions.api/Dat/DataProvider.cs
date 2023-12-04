using legallead.jdbc.entities;
using legallead.jdbc.interfaces;

namespace legallead.permissions.api
{
    public class DataProvider
    {
        private readonly IComponentRepository _componentDb;
        private readonly IPermissionMapRepository _permissionDb;
        private readonly IProfileMapRepository _profileDb;
        private readonly IUserPermissionRepository _userPermissionDb;
        private readonly IUserProfileRepository _userProfileDb;
        private readonly IUserRepository _userDb;

        internal DataProvider(
            IComponentRepository component,
            IPermissionMapRepository permissionDb,
            IProfileMapRepository profileDb,
            IUserPermissionRepository userPermissionDb,
            IUserProfileRepository userProfileDb,
            IUserRepository user)
        {
            _componentDb = component;
            _permissionDb = permissionDb;
            _profileDb = profileDb;
            _userPermissionDb = userPermissionDb;
            _userProfileDb = userProfileDb;
            _userDb = user;
        }

        internal IComponentRepository ComponentDb => _componentDb;
        internal IPermissionMapRepository PermissionDb => _permissionDb;
        internal IProfileMapRepository ProfileDb => _profileDb;
        internal IUserPermissionRepository UserPermissionDb => _userPermissionDb;
        internal IUserProfileRepository UserProfileDb => _userProfileDb;
        internal IUserRepository UserDb => _userDb;

        public async Task<bool> InitializeProfile(User user)
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
        public async Task<bool> InitializePermission(User user)
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

    }
}