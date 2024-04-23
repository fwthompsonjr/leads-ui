using legallead.email.models;
using Newtonsoft.Json;
using System.Text;

namespace legallead.email.services
{
    internal class ConnectionStringService : IConnectionStringService
    {
        private static readonly string SettingJs = Properties.Resources.db_settings;
        private const string SaltPhrase = "legal.lead.test.passcode";
        private const string ConnectCommandString = "server=<localhost>;user=<username>;password=<password>;Database=<database>;port=3306;";
        private readonly DbSetting dbSetting;
        private readonly ICryptographyService service;
        private string? decodedCredential;

        public ConnectionStringService(ICryptographyService cryptography)
        {
            var json = JsonConvert.DeserializeObject<DbSetting>(SettingJs) ?? new();
            dbSetting = json;
            service = cryptography;
        }


        public string[] GetCredential()
        {
            const char pipe = '|';
            try
            {
                if (string.IsNullOrEmpty(decodedCredential))
                {
                    string saltLocal = SaltPhrase;
                    string conversion = dbSetting.Code;
                    string vector = dbSetting.Key;
                    decodedCredential = service.Decrypt(conversion, saltLocal, vector);
                }
                return decodedCredential.Contains(pipe) ? decodedCredential.Split(pipe) : [];
            }
            catch (Exception)
            {
                return [];
            }
        }

        public string ConnectionString()
        {
            var passcode = GetCredential();
            if (passcode.Length == 0) return string.Empty;
            if (string.IsNullOrEmpty(dbSetting.Server)) return string.Empty;
            if (string.IsNullOrEmpty(dbSetting.DataBase)) return string.Empty;
            var connect = new StringBuilder(ConnectCommandString);
            connect.Replace("<localhost>", dbSetting.Server);
            connect.Replace("<database>", dbSetting.DataBase);
            connect.Replace("<username>", passcode[0]);
            connect.Replace("<password>", passcode[1]);
            return connect.ToString();
        }
    }
}
