using legallead.desktop.entities;

namespace legallead.desktop.interfaces
{
    internal interface IPermissionApi
    {
        IInternetStatus? InternetUtility { get; }

        ApiResponse CheckAddress(string name);

        KeyValuePair<bool, ApiResponse> CanGet(string name);

        KeyValuePair<bool, ApiResponse> CanPost(string name, object payload, UserBo user);
        Task<ApiResponse> Post(string name, object payload, UserBo user);
        Task<ApiResponse> Get(string name, UserBo user);
        Task<ApiResponse> Get(string name);
        Task<ApiResponse> Get(string name, Dictionary<string, string> parameters);
    }
}