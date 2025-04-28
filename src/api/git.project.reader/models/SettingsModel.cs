using git.project.reader.assets;
using Newtonsoft.Json;
using System.Text;

namespace git.project.reader.models
{
    public class SettingsModel
    {
        [JsonProperty("pat")] public string Pat { get; set; } = string.Empty;

        [JsonProperty("owner")] public string UserName { get; set; } = string.Empty;
        [JsonProperty("repo")] public string Repository { get; set; } = string.Empty;
        public SettingCode? Codes { get; set; }

        public string GetPat()
        {
            const string salt = "oxford.leg.test.passcode";
            if (Codes != null && Codes.IsValid())
            {
                var name = Codes.Name;
                var vector = Codes.Vector;
                var decoded = SecureCodeService.Decrypt(name, salt, vector);
                Pat = decoded;
                return SetToken(decoded, false);
            }
            if (string.IsNullOrWhiteSpace(Pat)) return "";
            return SetToken(Pat, true);
        }

        private static string _token = string.Empty;
        private static string SetToken(string pat, bool isEncoded)
        {
            if (!string.IsNullOrWhiteSpace(_token)) return _token;
            try
            {
                if (isEncoded)
                {
                    var bytes = Convert.FromBase64String(pat);
                    _token = Encoding.UTF8.GetString(bytes);
                    return _token;
                }
                else
                {
                    _token = pat;
                    return _token;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
