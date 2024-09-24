using legallead.permissions.api.Models;

namespace legallead.permissions.api.Interfaces
{
    public interface ISearchInfrastructure
    {
        Task<User?> GetUserAsync(HttpRequest request);

        Task<UserSearchBeginResponse?> BeginAsync(HttpRequest http, UserSearchRequest request);

        Task<IEnumerable<UserSearchDetail>?> GetStatusAsync(HttpRequest http, string? id);
        Task<IEnumerable<UserSearchDetail>?> GetDetailAsync(HttpRequest http, string? id);
        Task<IEnumerable<UserSearchDetail>?> GetResultAsync(HttpRequest http, string? id);
        Task<IEnumerable<UserSearchQueryModel>?> GetHeaderAsync(HttpRequest http, string? id);

        Task<bool> SetStatusAsync(string id, string message);

        Task<bool> SetDetailAsync(string id, object detail);

        Task<bool> SetResultAsync(string id, object result);
        Task<bool> SetResponseAsync(string id, object response);
        Task<IEnumerable<SearchPreviewBo>?> GetPreviewAsync(HttpRequest http, string searchId);
        Task<IEnumerable<SearchInvoiceBo>?> CreateInvoiceAsync(string userid, string searchid);
        Task<IEnumerable<SearchInvoiceBo>?> GetInvoicesAsync(string userid, string? searchid);
        Task<ActiveSearchOverviewBo?> GetSearchProgressAsync(string searchId);
        Task<object?> GetSearchDetailsAsync(string userId);
        Task<IEnumerable<PurchasedSearchBo>?> GetPurchasesAsync(string userId);
        Task<SearchRestrictionModel?> GetRestrictionStatusAsync(HttpRequest http);
        Task<bool> FlagErrorAsync(string searchId);
        Task<bool?> ExtendRestrictionAsync(HttpRequest http);
    }
}
