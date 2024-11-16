using legallead.jdbc.entities;
using Newtonsoft.Json;

namespace db.integration.tests
{
    internal static class TestAccountProvider
    {
        public static LeadUserDto GetChangePasswordAccount()
        {
            if (_changePasswordDto != null) return _changePasswordDto;
            lock (locker)
            {
                var tmp = JsonConvert.DeserializeObject<LeadUserDto>(changePasswordJson) ?? new();
                _changePasswordDto = tmp;
                return _changePasswordDto;
            }
        }

        private static LeadUserDto? _changePasswordDto = null;
        private static readonly string changePasswordJson = Properties.Resources.change_password_test;
        private static readonly object locker = new();
    }
}
