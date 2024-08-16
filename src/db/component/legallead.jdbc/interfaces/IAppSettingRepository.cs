using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IAppSettingRepository
    {
        AppSettingBo? FindKey(string keyName);
    }
}