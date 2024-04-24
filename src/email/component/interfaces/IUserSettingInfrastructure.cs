using legallead.email.models;

namespace legallead.email.interfaces
{
    internal interface IUserSettingInfrastructure
    {
        Task<List<UserEmailSettingBo>?> GetSettings(UserSettingQuery query);
    }
}