using legallead.desktop.entities;

namespace legallead.desktop.interfaces
{
    internal interface IHistoryReader
    {
        Task<string?> GetHistory(IPermissionApi? api, UserBo? user);
    }
}
