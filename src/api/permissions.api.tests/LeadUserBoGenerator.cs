using legallead.jdbc.entities;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Model;
using legallead.permissions.api.Models;
using legallead.permissions.api.Services;
using Newtonsoft.Json;
using System.Text;

namespace permissions.api.tests
{
    internal static class LeadUserBoGenerator
    {

        public static LeadUserBo GetBo(int countiesCount = 2, int indexCount = 0)
        {
            var thefaker = new Faker();
            var person = dtofaker.Generate();
            var uid = person.Id;
            var tokens = new List<LeadUserCountyDto>();
            var permissions = new List<LeadUserCountyIndexDto>{
                new ()
                {
                    Id = thefaker.Random.Guid().ToString("D"),
                    CreateDate = thefaker.Date.Recent(),
                    LeadUserId = uid,
                    CountyList = indexCount < 0 ? "-1" : GetNumericList(indexCount, thefaker)
                }
            };
            for (int i = 0; i < countiesCount; i++)
            {
                var countyName = thefaker.Address.County();
                var login = new { username = thefaker.Person.Email, password = thefaker.Random.AlphaNumeric(20) };
                var credential = $"{login.username}|{login.password}";
                var model = leadSvcs.CreateSecurityModel(credential);
                tokens.Add(new()
                {
                    Id = thefaker.Random.Guid().ToString("D"),
                    LeadUserId = uid,
                    CountyName = countyName,
                    CreateDate = thefaker.Date.Recent(),
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
        public static RegisterAccountModel GetAccount()
        {
            return faker.Generate();
        }

        public static UserCountyCredentialModel GetCountyLoginRequest()
        {
            return countyFaker.Generate();
        }

        public static UserCountyPermissionModel GetCountyPermissionRequest()
        {
            return countyListFaker.Generate();
        }


        public static UserCountyUsageModel GetCountyUsageRequest()
        {
            return countyLimitFaker.Generate();
        }

        public static string AppendSpecialCharacter(string source)
        {
            const string special = ".!#$%^*_+-=";
            var tmpFaker = new Faker();
            var charlist = special.ToCharArray();
            if (string.IsNullOrWhiteSpace(source)) return source;
            var items = source.ToCharArray();
            var builder = new StringBuilder();
            var additions = 0;
            var upperCase = 0;
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                if (!char.IsDigit(item) && upperCase < 2)
                {
                    builder.Append(tmpFaker.Lorem.Word()[..1].ToUpper());
                    upperCase++;
                }
                builder.Append(items[i]);
                if (additions > 2) continue;
                builder.Append(tmpFaker.PickRandom(charlist));
                additions++;
            }
            return builder.ToString();
        }

        private static readonly List<string> supportedCounties =
        [
            "collin",
            "tarrant",
            "dallas",
            "bexar",
            "hidalgo",
            "fort bend",
            "el paso",
            "travis",
            "denton"
        ];

        private static readonly Faker<RegisterAccountModel> faker =
            new Faker<RegisterAccountModel>()
            .RuleFor(x => x.UserName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Password, y => AppendSpecialCharacter(y.Random.AlphaNumeric(22)))
            .RuleFor(x => x.Email, y => y.Person.Email);

        private static readonly Faker<UserCountyCredentialModel> countyFaker =
            new Faker<UserCountyCredentialModel>()
            .RuleFor(x => x.UserName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Password, y => AppendSpecialCharacter(y.Random.AlphaNumeric(22)))
            .RuleFor(x => x.CountyName, y => y.PickRandom(supportedCounties));


        private static readonly Faker<UserCountyUsageModel> countyLimitFaker =
            new Faker<UserCountyUsageModel>()
            .RuleFor(x => x.UserName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.MonthlyUsage, y => y.Random.Int(0, 2500))
            .RuleFor(x => x.DateRange, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.CountyName, y => y.PickRandom(supportedCounties));

        private static readonly Faker<UserCountyPermissionModel> countyListFaker =
            new Faker<UserCountyPermissionModel>()
            .RuleFor(x => x.UserName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyList, y => y.Random.AlphaNumeric(22));


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
