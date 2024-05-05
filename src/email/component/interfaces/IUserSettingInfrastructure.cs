using legallead.email.entities;
using legallead.email.models;

namespace legallead.email.interfaces
{
    public interface IUserSettingInfrastructure
    {
        Task<List<UserEmailSettingBo>?> GetSettings(UserSettingQuery query);
        Task<UserAccountByEmailBo?> GetUserByEmail(string? email);
        Task<UserAccountByEmailBo?> GetUserBySearchId(string id);
        Task<LogCorrespondenceDto?> Log(string id, string json);
        void LogError(string id, string message);
        void LogSuccess(string id);
    }
}