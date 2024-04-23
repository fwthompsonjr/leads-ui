using Newtonsoft.Json;

namespace legallead.email.models
{
    internal class MailSettings
    {
        [JsonProperty("account")]
        public string Account { get; set; } = string.Empty;

        [JsonProperty("uid")]
        public string Uid { get; set; } = string.Empty;

        [JsonProperty("secret")]
        public string Secret { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string MailType { get; set; } = string.Empty;

        [JsonProperty("settings")]
        public SmtpSettings Settings { get; set; } = new();

        internal class SmtpSettings
        {
            [JsonProperty("endpoint")]
            public string Endpoint { get; set; } = string.Empty;

            [JsonProperty("port")]
            public int Port { get; set; }

            [JsonProperty("from")]
            public FromAccount From { get; set; } = new();
        }

        internal class FromAccount
        {
            [JsonProperty("email")]
            public string Email { get; set; } = string.Empty;

            [JsonProperty("display")]
            public string DisplayName { get; set; } = string.Empty;
        }
    }
}
/*
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class FromAccount
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("display")]
        public string DisplayName { get; set; }
    }

    public class MailSettings
    {
    }

    


*/