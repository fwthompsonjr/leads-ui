using legallead.jdbc.entities;
using Newtonsoft.Json;
using System.Text;

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

        public static LeadUserCountyDto GetChangeCountyAccount()
        {
            if (_changeCountyDto != null) return _changeCountyDto;
            lock (locker)
            {
                var builder = new StringBuilder(changeCounty);
                builder.Replace("'", '"'.ToString());
                builder.Replace("?", "'");
                var js = builder.ToString();
                var tmp = JsonConvert.DeserializeObject<LeadUserCountyDto>(js) ?? new();
                _changeCountyDto = tmp;
                return _changeCountyDto;
            }
        }

        private static LeadUserDto? _changePasswordDto = null;
        private static LeadUserCountyDto? _changeCountyDto = null;
        private static readonly string changePasswordJson = Properties.Resources.change_password_test;
        private static readonly object locker = new();
        private static readonly string changeCounty = "{" + Environment.NewLine +
        "'LeadUserId': '4411c3b7-a44d-11ef-99ce-0af7a01f52e9'," + Environment.NewLine +
        "'CountyName': 'collin'," + Environment.NewLine +
        "'Phrase': 'copying.the.card.won?t.d'," + Environment.NewLine +
        "'Vector': 'Lw50QBpMJxBQbQLgB4h6Dw=='," + Environment.NewLine +
        "'Token': 'MW1BphJIs2YFqhN7uuAwSeZB9/Rt7GgxKUrXjA0K+yY='," + Environment.NewLine +
        "'CreateDate': null," + Environment.NewLine +
        "'InsertFieldList': [" + Environment.NewLine +
        "'LeadUserId'," + Environment.NewLine +
        "'CountyName'," + Environment.NewLine +
        "'Phrase'," + Environment.NewLine +
        "'Vector'," + Environment.NewLine +
        "'Token'," + Environment.NewLine +
        "'CreateDate'," + Environment.NewLine +
        "'Id'" + Environment.NewLine +
        "]," + Environment.NewLine +
        "'UpdateFieldList': [" + Environment.NewLine +
        "'LeadUserId'," + Environment.NewLine +
        "'CountyName'," + Environment.NewLine +
        "'Phrase'," + Environment.NewLine +
        "'Vector'," + Environment.NewLine +
        "'Token'," + Environment.NewLine +
        "'CreateDate'," + Environment.NewLine +
        "'Id'" + Environment.NewLine +
        "]," + Environment.NewLine +
        "'Id': ''," + Environment.NewLine +
        "'TableName': 'LEADUSERCOUNTY'," + Environment.NewLine +
        "'FieldList': [" + Environment.NewLine +
        "'LeadUserId'," + Environment.NewLine +
        "'CountyName'," + Environment.NewLine +
        "'Phrase'," + Environment.NewLine +
        "'Vector'," + Environment.NewLine +
        "'Token'," + Environment.NewLine +
        "'CreateDate'," + Environment.NewLine +
        "'Id'" + Environment.NewLine +
        "]" + Environment.NewLine +
        "}";
    }
}
