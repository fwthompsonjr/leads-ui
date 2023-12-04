using legallead.jdbc.interfaces;

namespace legallead.permissions.api
{
    public class DataProvider
    {
        private readonly IComponentRepository _componentDb;
        private readonly IUserRepository _userDb;

        internal DataProvider(
            IComponentRepository component,
            IUserRepository user)
        {
            _componentDb = component;
            _userDb = user;
        }

        internal IComponentRepository ComponentDb => _componentDb;
        internal IUserRepository UserDb => _userDb;
    }
}