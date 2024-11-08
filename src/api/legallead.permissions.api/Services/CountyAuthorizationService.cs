using legallead.permissions.api.Extensions;
using legallead.permissions.api.Models;

namespace legallead.permissions.api.Services
{
    public class CountyAuthorizationService : ICountyAuthorizationService
    {
        public List<AuthorizedCountyModel> Models => GetModels();

        private static List<AuthorizedCountyModel> GetModels()
        {
            var tmp = modeljs.ToInstance<List<AuthorizedCountyModel>>() ?? [];
            return tmp;
        }

        private static readonly string modeljs = Properties.Resources.county_code_definition;
    }
}
