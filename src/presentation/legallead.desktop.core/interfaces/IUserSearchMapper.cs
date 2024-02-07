using legallead.desktop.entities;

namespace legallead.desktop.interfaces
{
    internal interface IUserSearchMapper
    {
        Task<string> Map(IPermissionApi api, UserBo user, string source, string target);
    }
}
