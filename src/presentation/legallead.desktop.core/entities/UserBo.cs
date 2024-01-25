using Newtonsoft.Json;

namespace legallead.desktop.entities
{
    internal class UserBo
    {
        private AccessTokenBo? token;

        public virtual bool IsAuthenicated => Token != null && Token.Expires.HasValue && !IsExpired(Token.Expires.Value);
        public string UserName { get; set; } = string.Empty;
        public string SessionId { get; private set; } = string.Empty;
        public ApiContext[]? Applications { get; set; }
        public bool IsInitialized => Applications != null && Applications.Length > 0;

        public AccessTokenBo? Token
        {
            get { return token; }
            internal set
            {
                token = value;
                if( token == null || string.IsNullOrEmpty(token.AccessToken))
                {
                    SessionId = "-unset-";
                }
                else
                {
                     SessionId = Guid.NewGuid().ToString().Split('-')[^1];
                }
                AuthenicatedChanged?.Invoke();
            }
        }

        public Action? AuthenicatedChanged { get; internal set; }

        public string GetAppServiceHeader()
        {
            var count = Applications?.Length ?? 0;
            if (count <= 0 || Applications == null) return string.Empty;
            var item = Applications[0];
            return JsonConvert.SerializeObject(item);
        }

        private static bool IsExpired(DateTime expires)
        {
            var difference = expires - DateTime.UtcNow;
            return difference.TotalMinutes > 1;
        }
    }
}