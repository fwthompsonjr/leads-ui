using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Services;

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

        public Task<string> GetCountyCredentialAsync(string userId, string county)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetCountyPermissionAsync(string userId)
        {
            throw new NotImplementedException();
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
