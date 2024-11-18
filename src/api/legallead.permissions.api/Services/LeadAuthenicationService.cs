using legallead.jdbc.interfaces;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Extensions;
using Newtonsoft.Json;

namespace legallead.permissions.api.Services
{
    public class LeadAuthenicationService(ILeadUserRepository repo, ILeadSecurityService svc) : ILeadAuthenicationService
    {
        private readonly ILeadUserRepository _repo = repo;
        private readonly ILeadSecurityService _svc = svc;


        public KeyValuePair<bool, string> VerifyCountyList(string countyList)
        {
            const char comma = ',';
            var ispassed = true;
            var reason = string.Empty;
            var fallback = new KeyValuePair<bool, string>(ispassed, reason);
            if (string.IsNullOrWhiteSpace(countyList))
            {
                ispassed = false;
                reason = "County list is blank";
                return new KeyValuePair<bool, string>(ispassed, reason);
            }
            if (countyList.Equals("-1"))
            {
                return fallback;
            }
            var entries = countyList.Split(comma, StringSplitOptions.RemoveEmptyEntries).ToList();
            var hasnonumeric = entries.Exists(x => !int.TryParse(x, out var value));
            if (hasnonumeric)
            {
                ispassed = false;
                reason = "County list contains non-numeric value.";
                return new KeyValuePair<bool, string>(ispassed, reason);
            }
            var values = Enum.GetValues(typeof(SourceType)).Cast<int>().ToList();
            var numerics = entries
                .FindAll(x => int.TryParse(x, out var value))
                .Select(x => Convert.ToInt32(x))
                .Where(w => !values.Contains(w))
                .ToList();

            if (numerics.Count > 0)
            {
                ispassed = false;
                reason = "County list contains invalid county index value.";
                return new KeyValuePair<bool, string>(ispassed, reason);
            }

            return fallback;
        }

        public async Task<bool> AddCountyUsageAsync(string userId, string county, int monthlyUsage)
        {
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(county) ||
                monthlyUsage > 100000) return false;
            var dto = new LeadUserCountyDto
            {
                LeadUserId = userId,
                CountyName = county,
                MonthlyUsage = monthlyUsage
            };

            var response = await _repo.AddCountyUsage(dto);
            return response;
        }

        public async Task<bool> AddCountyUsageIncidentAsync(string userId, string county, int monthlyUsage)
        {
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(county) ||
                monthlyUsage > 100000) return false;
            var dto = new LeadUserCountyDto
            {
                LeadUserId = userId,
                CountyName = county,
                MonthlyUsage = monthlyUsage
            };

            var response = await _repo.AppendUsageIncident(dto);
            return response;
        }
        public async Task<bool> ChangeCountyCredentialAsync(string userId, string county, string userName, string password)
        {
            var model = await GetLeadUserModelAsync(userId);
            if (model == null) return false;
            var current = await GetCountyCredentialAsync(userId, county);
            var isNew = string.IsNullOrEmpty(current) || current.Equals("[]");
            var dto = GetCountyDto(model, current, county, userName, password);
            return isNew ?
                await _repo.AddCountyToken(dto) :
                await _repo.UpdateCountyToken(dto);
        }

        public async Task<bool> ChangeCountyPermissionAsync(string userId, string countyList)
        {
            var model = await GetLeadUserModelAsync(userId);
            if (model == null) return false;
            var current = await GetCountyPermissionAsync(userId);
            var isNew = string.IsNullOrEmpty(current);
            var dto = GetIndexDto(model, current, countyList);
            return isNew ?
                await _repo.AddCountyPermissions(dto) :
                await _repo.UpdateCountyPermissions(dto);
        }

        public async Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            var model = await GetLeadUserModelAsync(userId);
            if (model == null) return false;
            if (string.IsNullOrEmpty(CheckCredential(model, oldPassword))) return false;
            var userName = model.UserName;
            var dto = new LeadUserDto
            {
                Id = userId,
                UserName = userName,
            };
            SetDtoCredential(dto, userName, newPassword);
            var response = await _repo.UpdateAccount(dto);
            if (!response) return false;
            var verification = await LoginAsync(userName, newPassword);
            return !string.IsNullOrEmpty(verification);
        }

        public async Task<string> CreateLoginAsync(string userName, string password, string email)
        {
            var dto = new LeadUserDto
            {
                UserName = userName,
                Email = email,
            };
            SetDtoCredential(dto, userName, password);
            var response = await _repo.AddAccount(dto);
            return string.IsNullOrWhiteSpace(response) ? string.Empty : response;
        }

        public async Task<string> GetCountyCredentialAsync(string userId, string county)
        {
            var model = await GetLeadUserModelAsync(userId);
            if (model == null) return string.Empty;
            var json = _svc.GetCountyData(model);
            var data = json.ToInstance<List<LeadUserCountyDto>>();
            if (data == null) return string.Empty;
            var requested = data.FindAll(x => x.CountyName == county);
            return JsonConvert.SerializeObject(requested);
        }

        public async Task<string> GetCountyPermissionAsync(string userId)
        {
            var model = await GetLeadUserModelAsync(userId);
            if (model == null) return string.Empty;
            return _svc.GetCountyPermissionData(model);
        }
        public async Task<List<GetUsageUserByIdResponse>> GetUsageUserByIdAsync(string userId)
        {
            var list = new List<GetUsageUserByIdResponse>();
            var response = await _repo.GetUsageUserById(userId);
            var items = response.Select(s => new GetUsageUserByIdResponse
            {
                CountyName = s.CountyName,
                MonthlyUsage = s.MonthlyUsage.GetValueOrDefault(),
                CreateDate = s.CreateDate,
            });
            list.AddRange(items);
            return list;
        }
        public async Task<string> LoginAsync(string userName, string password)
        {
            var model = await GetLeadUserModelAsync(userName, false);
            if (model == null) return string.Empty;
            if (string.IsNullOrEmpty(CheckCredential(model, password))) return string.Empty;
            return JsonConvert.SerializeObject(model);
        }

        public async Task<LeadUserModel?> GetModelByIdAsync(string id)
        {
            return await GetLeadUserModelAsync(id);
        }

        public LeadUserModel? GetUserModel(HttpRequest? request, string reason)
        {
            if (request == null) return null;
            var tmp = request.GetObjectFromHeader<string>("LEAD_IDENTITY");
            if (tmp == null) return null;
            var validations = LeadTokenService.GetValidationModel(tmp, reason);
            if (!validations.Validated) return null;
            var identity = LeadTokenService.GetModel(tmp, out var _);
            if (identity == null) return null;
            return identity.User;
        }
        private string CheckCredential(LeadUserModel model, string password)
        {
            var user = _svc.GetUserData(model);
            var dto = user.ToInstance<LeadUserDto>();
            if (dto == null ||
                string.IsNullOrEmpty(dto.Token) ||
                string.IsNullOrEmpty(dto.Phrase) ||
                string.IsNullOrEmpty(dto.Vector)) return string.Empty;
            var decoded = secureSvcs.Decrypt(dto.Token, dto.Phrase, dto.Vector);
            if (string.IsNullOrEmpty(decoded) || !decoded.Contains('|')) return string.Empty;
            if (decoded.Split('|')[^1].Equals(password)) return dto.Id;
            return string.Empty;
        }

        private LeadUserCountyDto GetCountyDto(LeadUserModel model, string json, string county, string userName, string password)
        {
            var dto = json.ToInstance<LeadUserCountyDto>() ?? new();
            dto.LeadUserId = model.Id;
            dto.CountyName = county;
            SetDtoCredential(dto, userName, password);
            return dto;
        }

        private void SetDtoCredential(BaseDto dto, string userName, string password)
        {
            var credentials = $"{userName}|{password}";
            var obj = _svc.CreateSecurityModel(credentials);
            dto["Phrase"] = obj.Phrase;
            dto["Vector"] = obj.Vector;
            dto["Token"] = obj.Token;
        }

        private static LeadUserCountyIndexDto GetIndexDto(LeadUserModel model, string json, string countyList)
        {
            var dto = json.ToInstance<LeadUserCountyIndexDto>() ?? new();

            dto.CountyList = LeadSecurityService.ToIntegerString(countyList);
            dto.LeadUserId = model.Id;
            return dto;
        }

        private async Task<LeadUserModel?> GetLeadUserModelAsync(string identity, bool byIndex = true)
        {
            var bo = byIndex ?
                await _repo.GetUserById(identity) :
                await _repo.GetUser(identity);
            if (bo == null) return null;
            return _svc.GetModel(bo);
        }

        private static readonly SecureStringService secureSvcs = new();
        private enum SourceType
        {
            DentonCounty = 1,
            CollinCounty = 20,
            TarrantCounty = 10,
            HarrisCivil = 30,
            HarrisCriminal = 40,
            DallasCounty = 60,
            TravisCounty = 70,
            BexarCounty = 80,
            HidalgoCounty = 90,
            ElPasoCounty = 100,
            FortBendCounty = 110,
            WilliamsonCounty = 120,
            GraysonCounty = 130,
        }
    }
}
