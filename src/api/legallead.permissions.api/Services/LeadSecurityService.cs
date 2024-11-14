using Bogus;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Extensions;
using Newtonsoft.Json;
using System.Globalization;

namespace legallead.permissions.api.Services
{
    public class LeadSecurityService : ILeadSecurityService
    {
        public LeadUserSecurityModel CreateSecurityModel(string cleartext)
        {
            var passcode = GetPassPhrase();
            var encoded = secureSvcs.Encrypt(cleartext, passcode, out var vector);
            return new LeadUserSecurityModel
            {
                Phrase = passcode,
                Token = encoded,
                Vector = vector
            };
        }

        public string GetCountyData(LeadUserModel model)
        {
            var data = model.CountyData.ToInstance<List<LeadUserCountyDto>>();
            if (data == null || data.Count == 0) return string.Empty;
            var items = data.Select(d =>
            {
                return new { d.LeadUserId, d.CountyName, Model = GetDecodedString(MapFrom(d)) };
            });
            return JsonConvert.SerializeObject(items);
        }

        public string GetCountyPermissionData(LeadUserModel model)
        {
            var data = model.IndexData.ToInstance<List<LeadUserCountyIndexDto>>();
            if (data == null || data.Count == 0) return string.Empty;
            var items = data.Select(s =>
            {
                var counties = s.CountyList ?? string.Empty;
                return new { s.LeadUserId, CountyList = ToIntegerString(counties) };
            });
            return JsonConvert.SerializeObject(items);
        }

        public LeadUserModel GetModel(LeadUserBo user)
        {
            var model = new LeadUserModel
            {
                Id = user.Id,
                UserName = user.UserName,
                CountyData = user.CountyData,
                IndexData = user.IndexData,
                UserData = user.UserData,
            };
            model.CountyData = GetCountyData(model);
            model.IndexData = GetCountyPermissionData(model);
            return model;
        }

        public string GetPassPhrase()
        {
            const int mx = 24;
            var fkr = new Faker();
            var token = string.Empty;
            while (token.Length < mx)
            {
                token = fkr.Hacker.Phrase().ToLower(CultureInfo.CurrentCulture);
            }
            token = token.Substring(0, mx);
            return string.Join('.', token.Split(' '));
        }

        public string GetUserData(LeadUserModel model)
        {
            var data = model.UserData.ToInstance<LeadUserDto>();
            if (data == null) return string.Empty;
            return model.UserData;
        }

        private static string GetDecodedString(LeadUserSecurityModel model)
        {
            if (string.IsNullOrEmpty(model.Token)) return string.Empty;
            if (string.IsNullOrEmpty(model.Phrase)) return string.Empty;
            if (string.IsNullOrEmpty(model.Vector)) return string.Empty;
            var decoded = secureSvcs.Decrypt(model.Token, model.Phrase, model.Vector);
            return decoded;
        }
        public static string ToIntegerString(string? countyList)
        {
            const char comma = ',';
            if (string.IsNullOrWhiteSpace(countyList)) return string.Empty;
            if (!countyList.Contains(comma))
            {
                if (!int.TryParse(countyList, out var _)) return string.Empty;
                return countyList;
            }
            var arr = countyList.Split(comma);
            var numbers = arr.Where(c => int.TryParse(c, out var _));
            return string.Join(comma, numbers);
        }
        private static LeadUserSecurityModel MapFrom(LeadUserCountyDto dto)
        {
            return new()
            {
                Token = dto.Token,
                Phrase = dto.Phrase,
                Vector = dto.Vector
            };
        }
        private static readonly SecureStringService secureSvcs = new();
    }
}
