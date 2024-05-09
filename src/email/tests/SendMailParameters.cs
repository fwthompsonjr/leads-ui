namespace legallead.email.tests
{
    internal class SendMailParameters
    {
        [JsonProperty("uid")]
        public string UserId { get; set; } = string.Empty;
        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;
    }
}
