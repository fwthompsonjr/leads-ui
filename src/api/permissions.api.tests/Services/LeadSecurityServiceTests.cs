using legallead.jdbc.entities;
using legallead.permissions.api.Services;
using Newtonsoft.Json;

namespace permissions.api.tests.Services
{
    public class LeadSecurityServiceTests
    {
        [Fact]
        public void SerivceCanCreateSecurityModel()
        {
            var error = Record.Exception(() =>
            {
                _ = dtofaker.Generate();
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0, 5)]
        [InlineData(1, 5)]
        [InlineData(2, 5)]
        [InlineData(0, -1)]
        [InlineData(0, 1)]
        [InlineData(0, 2)]
        public void SerivceCanGetModel(int counties, int indexes)
        {
            var error = Record.Exception(() =>
            {
                var bo = GetBo(counties, indexes);
                var model = leadSvcs.GetModel(bo);
                Assert.False(string.IsNullOrEmpty(model.Id));
            });
            Assert.Null(error);
        }

        [Fact]
        public void SerivceCanGetUserData()
        {
            var error = Record.Exception(() =>
            {
                var bo = GetBo();
                var userData = leadSvcs.GetUserData(leadSvcs.GetModel(bo));
                Assert.False(string.IsNullOrEmpty(userData));
            });
            Assert.Null(error);
        }

        private static LeadUserBo GetBo(int countiesCount = 2, int indexCount = 0)
        {
            var faker = new Faker();
            var person = dtofaker.Generate();
            var uid = person.Id;
            var tokens = new List<LeadUserCountyDto>();
            var permissions = new List<LeadUserCountyIndexDto>{
                new ()
                {
                    Id = faker.Random.Guid().ToString("D"),
                    CreateDate = faker.Date.Recent(),
                    LeadUserId = uid,
                    CountyList = indexCount < 0 ? "-1" : GetNumericList(indexCount, faker)
                }
            };
            for (int i = 0; i < countiesCount; i++)
            {
                var countyName = faker.Address.County();
                var login = new { username = faker.Person.Email, password = faker.Random.AlphaNumeric(20) };
                var credential = $"{login.username}|{login.password}";
                var model = leadSvcs.CreateSecurityModel(credential);
                tokens.Add(new()
                {
                    Id = faker.Random.Guid().ToString("D"),
                    LeadUserId = uid,
                    CountyName = countyName,
                    CreateDate = faker.Date.Recent(),
                    Phrase = model.Phrase,
                    Token = model.Token,
                    Vector = model.Vector,
                });
            }
            return new LeadUserBo
            {
                Id = uid,
                CountyData = JsonConvert.SerializeObject(tokens),
                UserData = JsonConvert.SerializeObject(person),
                IndexData = JsonConvert.SerializeObject(permissions),
            };
        }


        private static string GetNumericList(int indexCount, Faker faker)
        {
            var list = new List<int>();
            for (var c = 0; c < indexCount; c++)
            {
                list.Add(faker.Random.Int(1, 50));
            }
            return string.Join(',', list);
        }
        private static readonly LeadSecurityService leadSvcs = new();
        private readonly static Faker<LeadUserDto> dtofaker =
            new Faker<LeadUserDto>()
            .FinishWith((a, b) =>
            {
                var cleartext = a.Hacker.Phrase();
                var model = leadSvcs.CreateSecurityModel(cleartext);
                b.Id = a.Random.Guid().ToString("D");
                b.UserName = a.Person.UserName;
                b.CreateDate = a.Date.Recent();
                b.Token = model.Token;
                b.Phrase = model.Phrase;
                b.Vector = model.Vector;
            });
    }
}
