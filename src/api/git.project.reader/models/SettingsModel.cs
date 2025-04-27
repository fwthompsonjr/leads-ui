using Newtonsoft.Json;
using System.Text;

namespace git.project.reader.models
{
    public class SettingsModel
    {
        [JsonProperty("pat")] public string Pat { get; set; } = string.Empty;

        [JsonProperty("owner")] public string UserName { get; set; } = string.Empty;
        [JsonProperty("repo")] public string Repository { get; set; } = string.Empty;

        public string GetPat()
        {
            if (string.IsNullOrWhiteSpace(Pat)) return "";
            return SetToken(Pat);
        }

        private static string _token = string.Empty;
        private static string SetToken(string pat)
        {
            if (!string.IsNullOrWhiteSpace(_token)) return _token;
            try
            {
                var bytes = Convert.FromBase64String(pat);
                _token = Encoding.UTF8.GetString(bytes);
                return _token;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
