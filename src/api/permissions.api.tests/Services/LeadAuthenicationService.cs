using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Services;
using Newtonsoft.Json;
using StructureMap.Query;

namespace permissions.api.tests.Services
{
    public class LeadAuthenicationService(ILeadUserRepository repo, ILeadSecurityService svc) : ILeadAuthenicationService
    {
        private readonly ILeadUserRepository _repo = repo;
        private readonly ILeadSecurityService _svc = svc;
        public Task<bool> ChangeCountyCredentialAsync(string userId, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangeCountyPermissionAsync(string userId, string countyList)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CreateLoginAsync(string userName, string password)
        {
            var credentials = $"{userName}|{password}";
            var model = _svc.CreateSecurityModel(credentials);
            var dto = new LeadUserDto
            {
                Phrase = model.Phrase,
                UserName = userName,
                Token = model.Token,
                Vector = model.Vector,
            };
            var response = await _repo.AddAccount(dto);
            return response ?? string.Empty;
        }

        public async Task<string> GetCountyCredentialAsync(string userId, string county)
        {
            var bo = await _repo.GetUserById(userId);
            if (bo == null) return string.Empty;
            var model = _svc.GetModel(bo);
            var json = _svc.GetCountyData(model);
            var data = json.ToInstance<List<LeadUserCountyDto>>();
            if (data == null) return string.Empty;
            var requested = data.FindAll(x => x.CountyName == county);
            return JsonConvert.SerializeObject(requested);
        }

        public async Task<string> GetCountyPermissionAsync(string userId)
        {
            var bo = await _repo.GetUserById(userId);
            if (bo == null) return string.Empty;
            var model = _svc.GetModel(bo);
            return _svc.GetCountyPermissionData(model);
        }

        public async Task<string> LoginAsync(string userName, string password)
        {
            var bo = await _repo.GetUser(userName);
            if (bo == null) return string.Empty;
            var model = _svc.GetModel(bo);
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
        private static readonly SecureStringService secureSvcs = new();
    }
}
