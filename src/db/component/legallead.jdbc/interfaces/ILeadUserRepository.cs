using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface ILeadUserRepository
    {
        Task<LeadUserBo?> GetUser(string userName);

        Task<string> AddAccount(LeadUserDto user);
        Task<bool> AddCountyToken(LeadUserCountyDto userCounty);
        Task<bool> AddCountyPermissions(LeadUserCountyIndexDto userPermissions);

        Task<bool> UpdateAccount(LeadUserDto user);
        Task<bool> UpdateCountyToken(LeadUserCountyDto userCounty);
        Task<bool> UpdateCountyPermissions(LeadUserCountyIndexDto userPermissions);
        Task<LeadUserBo?> GetUserById(string userId);
        Task<bool> AddCountyUsage(LeadUserCountyDto userCounty);
        Task<List<LeadUserCountyUsageDto>> GetUsageUserById(string userId);
        Task<bool> AppendUsageIncident(LeadUserCountyDto dto);
    }
}
