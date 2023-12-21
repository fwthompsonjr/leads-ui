using Newtonsoft.Json;

namespace legallead.desktop.entities
{
    internal class UserBo
    {
        public bool IsAuthenicated { get; set; }
        public string UserName { get; set; } = string.Empty;
        public ApiContext[]? Applications { get; set; }
        public bool IsInitialized => Applications != null;

        public string GetAppServiceHeader()
        {
            var count = Applications?.Length ?? 0;
            if (count <= 0 || Applications == null) return string.Empty;
            var item = Applications[0];
            return JsonConvert.SerializeObject(item);
        }
    }
}