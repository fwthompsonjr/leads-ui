using legallead.permissions.api.Models;

namespace legallead.permissions.api.Interfaces
{
    public interface ISearchInfrastructure
    {
        Task<User?> GetUser(HttpRequest request);

        Task<UserSearchBeginResponse?> Begin(HttpRequest http, UserSearchRequest request);

        Task<IEnumerable<UserSearchDetail>?> GetStatus(HttpRequest http, string? id);
        Task<IEnumerable<UserSearchDetail>?> GetDetail(HttpRequest http, string? id);
        Task<IEnumerable<UserSearchDetail>?> GetResult(HttpRequest http, string? id);
        Task<IEnumerable<UserSearchQueryModel>?> GetHeader(HttpRequest http, string? id);

        Task<bool> SetStatus(string id, string message);

        Task<bool> SetDetail(string id, object detail);

        Task<bool> SetResult(string id, object result);
        Task<bool> SetResponse(string id, object response);
        Task<IEnumerable<SearchPreviewBo>?> GetPreview(HttpRequest http, string searchId);
        Task<IEnumerable<SearchInvoiceBo>?> CreateInvoice(string userid, string searchid);
        Task<IEnumerable<SearchInvoiceBo>?> GetInvoices(string userid, string? searchid);
        Task<ActiveSearchOverviewBo?> GetSearchProgress(string searchId);
        Task<object?> GetSearchDetails(string userId);
        Task<IEnumerable<PurchasedSearchBo>?> GetPurchases(string userId);
        Task<SearchRestrictionModel?> GetRestrictionStatus(HttpRequest http);
        Task<bool> FlagError(string searchId);
    }
}
