using Newtonsoft.Json;

namespace legallead.desktop.entities
{
    internal class UserBo
    {
        private AccessTokenBo? token;

        public virtual bool IsAuthenicated => Token != null && Token.Expires.HasValue;
        public string UserName { get; set; } = string.Empty;
        public ApiContext[]? Applications { get; set; }
        public bool IsInitialized => Applications != null && Applications.Length > 0;

        public AccessTokenBo? Token
        {
            get { return token; }
            internal set
            {
                token = value;
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
    }
}