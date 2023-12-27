
namespace legallead.permissions.api.Model
{
    public class Tokens
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? Expires { get; internal set; }
    }
}