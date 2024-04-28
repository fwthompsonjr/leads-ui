using legallead.email.models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace legallead.email.services
{
    internal class ConnectionStringService(ICryptographyService cryptography) : IConnectionStringService
    {
        private static readonly string SettingJs = Properties.Resources.db_settings;
        private const string SaltPhrase = "legal.lead.test.passcode";
        private const string ConnectCommandString = "server=<localhost>;user=<username>;password=<password>;Database=<database>;port=3306;";
        private readonly DbSetting dbSetting = GetSetting();
        private readonly ICryptographyService service = cryptography;
        private string? decodedCredential;

        public string[] GetCredential()
        {
            try
            {
                if (string.IsNullOrEmpty(decodedCredential))
                {
                    string saltLocal = SaltPhrase;
                    string conversion = dbSetting.Code;
                    string vector = dbSetting.Key;
                    decodedCredential = service.Decrypt(conversion, saltLocal, vector);
                }
                return Decode(decodedCredential);
            }
            catch (Exception)
            {
                return [];
            }
        }

        public string ConnectionString()
        {
            var passcode = GetCredential();
            if (!VerifyCredential(passcode)) return string.Empty;
            var connect = new StringBuilder(ConnectCommandString);
            connect.Replace("<localhost>", dbSetting.Server);
            connect.Replace("<database>", dbSetting.DataBase);
            connect.Replace("<username>", passcode[0]);
            connect.Replace("<password>", passcode[1]);
            return connect.ToString();
        }

        [ExcludeFromCodeCoverage(Justification = "Private utility tested from public member.")]
        private string[] Decode(string input)
        {
            const char pipe = '|';
            return input.Contains(pipe) ? input.Split(pipe) : [];
        }

        [ExcludeFromCodeCoverage(Justification = "Private utility tested from public member.")]
        private bool VerifyCredential(string[] passcode)
        {
            if (passcode.Length == 0) return false;
            if (string.IsNullOrEmpty(dbSetting.Server)) return false;
            if (string.IsNullOrEmpty(dbSetting.DataBase)) return false;
            return true;
        }

        [ExcludeFromCodeCoverage(Justification = "Private utility tested from public member.")]
        private static DbSetting GetSetting()
        {
            try
            {
                return JsonConvert.DeserializeObject<DbSetting>(SettingJs) ?? new();
            }
            catch
            {
                return new();
            }
        }
    }
}
