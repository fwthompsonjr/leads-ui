using legallead.email.entities;
using legallead.email.models;

namespace legallead.email.interfaces
{
    public interface IUserSettingInfrastructure
    {
        Task<List<UserEmailSettingBo>?> GetSettings(UserSettingQuery query);
        Task<LogCorrespondenceDto?> Log(string id, string json);
    }
}