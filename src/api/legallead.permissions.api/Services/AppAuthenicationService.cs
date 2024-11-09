using legallead.permissions.api.Models;
using Newtonsoft.Json;

namespace legallead.permissions.api.Services
{
    public class AppAuthenicationService : IAppAuthenicationService
    {
        public AppAuthenicationItemDto? Authenicate(string userName, string password)
        {
            const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return null;
            return UserList.Find(x => x.UserName.Equals(userName, comparison) && x.Code.Equals(password));
        }
        public AppAuthenicationItemDto? FindUser(string userName, int id)
        {
            const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            if (string.IsNullOrEmpty(userName)) return null;
            var dto = UserList.Find(x => x.UserName.Equals(userName, comparison) && x.Id.Equals(id));
            if (dto == null) return null;
            return new AppAuthenicationItemDto { Id = dto.Id, UserName = userName };
        }

        private static List<AppAuthenicationItemDto> UserList
        {
            get
            {
                if (list != null) return list;
                lock (locker)
                {
                    var dto = JsonConvert.DeserializeObject<AppAuthenicationListDto>(userlist) ?? new();
                    var service = new SecureStringService();
                    var data = service.Decrypt(dto.Data, dto.Code, dto.Vector);
                    var tmp = JsonConvert.DeserializeObject<List<string>>(data) ?? [];
                    var decodedList = tmp.Select(SecureStringService.FromBase64);
                    list = decodedList
                        .Select(JsonConvert.DeserializeObject<AppAuthenicationItemDto>)
                        .Where(x => x != null)
                        .Cast<AppAuthenicationItemDto>()
                        .ToList();
                    return list; 
                }
            }
        }
        private static List<AppAuthenicationItemDto>? list = null;
        private static readonly string userlist = Properties.Resources.user_authentication_list;
        private static readonly object locker = new();
    }
}
