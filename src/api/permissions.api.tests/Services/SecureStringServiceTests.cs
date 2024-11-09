using legallead.permissions.api.Models;
using legallead.permissions.api.Services;
using Newtonsoft.Json;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace permissions.api.tests.Services
{
    public class SecureStringServiceTest
    {
        [Fact]
        public void ServiceCanBeContructed()
        {
            var service = new SecureStringService();
            Assert.NotNull(service);
        }

        [Fact]
        public void ServiceCanEncyrpt()
        {
            const string passphrase = "internal.testing.psscode";
            var text = new Faker().Hacker.Phrase();
            var service = new SecureStringService();
            var encoded = service.Encrypt(text, passphrase, out var vector);
            var decoded = service.Decrypt(encoded, passphrase, vector);
            Assert.Equal(text, decoded);
        }

        [Fact]
        public void ServiceCanEncodeRandomList()
        {
            const string passphrase = "internal.testing.psscode";
            var text = GetRandomList();
            var service = new SecureStringService();
            var encoded = service.Encrypt(text, passphrase, out var vector);
            var decoded = service.Decrypt(encoded, passphrase, vector);
            Assert.Equal(text, decoded);
        }


        [Fact]
        public void ServiceCanGenerateUserList()
        {
            var text = GenerateUserList();
            Assert.False(string.IsNullOrEmpty(text));
        }


        [Fact]
        public void ServiceCanDeserializeUserList()
        {
            var text = GenerateUserList();
            var dto = JsonConvert.DeserializeObject<AppAuthenicationListDto>(text);
            Assert.NotNull(dto);
            var service = new SecureStringService();
            var data = service.Decrypt(dto.Data, dto.Code, dto.Vector);
            Assert.False(string.IsNullOrEmpty(data));
            var list = JsonConvert.DeserializeObject<List<string>>(data) ?? [];
            var decodedList = list.Select(SecureStringService.FromBase64);
            Assert.NotEmpty(decodedList);
            var items = decodedList.Select(d => JsonConvert.DeserializeObject<AppAuthenicationItemDto>(d));
            Assert.NotEmpty(items);
            Assert.DoesNotContain(items, i => i == null);
        }

        private static string GetRandomList()
        {
            var faker = new Faker();
            var list = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var obj = new { userName = faker.Person.UserName, secret = faker.Random.AlphaNumeric(12) };
                var js = SecureStringService.GetKeyBase64(JsonConvert.SerializeObject(obj));
                list.Add(js);
            }
            return JsonConvert.SerializeObject(list);
        }
        private static string GenerateUserList()
        {
            const string listName = "user.authentication.list";
            var faker = new Faker();
            var list = new List<string>() { 
                "lead.administrator",
                "Kerri@kdphillipslaw.com" 
            };
            var service = new SecureStringService();
            for (int i = 0; i < list.Count; i++)
            {
                var dto = UserDtoService.GetItemById(i);
                var pword = dto?.UserCode ?? faker.Random.AlphaNumeric(12);
                var obj = new { id = i, userName = list[i], secret = pword };
                var js = SecureStringService.GetKeyBase64(JsonConvert.SerializeObject(obj));
                list[i] = js;
            }
            var data = service.Encrypt(JsonConvert.SerializeObject(list), listName, out var vector);
            var final = new { keyCode = listName, vector, data };
            return JsonConvert.SerializeObject(final);
        }


        private sealed class InternalUserDto
        {
            public int Id { get; set; } = -1;
            [JsonProperty("code")]
            public string UserCode { get; set; } = string.Empty;
        }
        private static class UserDtoService
        {
            public static InternalUserDto? GetItemById(int id)
            {
                return GetList.Find(x => x.Id == id);
            }
            private static List<InternalUserDto> GetList
            {
                get
                {
                    if (list != null) return list;
                    list = JsonConvert.DeserializeObject<List<InternalUserDto>>(userjs) ?? [];
                    return list;
                }
            }
            private static List<InternalUserDto>? list = default;
            private static readonly string userjs = Properties.Resources.internal_user_secrets;
        }
    }
}