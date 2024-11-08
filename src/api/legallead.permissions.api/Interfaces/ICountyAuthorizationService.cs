using legallead.permissions.api.Models;

namespace legallead.permissions.api.Interfaces
{
    public interface ICountyAuthorizationService
    {
        public List<AuthorizedCountyModel> Models { get; }
    }
}
