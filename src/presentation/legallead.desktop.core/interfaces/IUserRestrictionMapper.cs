using legallead.desktop.entities;

namespace legallead.desktop.interfaces
{
    internal interface IUserRestrictionMapper
    {
        Task<string> Map(IPermissionApi api, UserBo user, string source);
    }
}
