using legallead.jdbc.interfaces;

namespace legallead.permissions.api.Services
{
    public class AppSettingService(IAppSettingRepository repo) : IAppSettingService
    {
        private readonly IAppSettingRepository _repo = repo;

        public string? FindKey(string keyName)
        {
            var response = _repo.FindKey(keyName);
            if (response == null || string.IsNullOrWhiteSpace(response.KeyValue)) return null;
            return response.KeyValue;
        }
    }
}
