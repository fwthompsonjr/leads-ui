﻿using legallead.jdbc.entities;
using legallead.jdbc.models;

namespace legallead.jdbc.interfaces
{
    public interface IUserSearchRepository
    {
        Task<KeyValuePair<bool, string>> Append(SearchTargetTypes search, string? id, object data, string? keyname = null);
        Task<KeyValuePair<bool, string>> Begin(string userId, string payload);
        Task<KeyValuePair<bool, string>> Complete(string id);
        Task<IEnumerable<SearchTargetModel>?> GetTargets(SearchTargetTypes search, string? userId, string? id);
        Task<IEnumerable<SearchDtoHeader>> History(string userId);
        Task<IEnumerable<SearchPreviewBo>> Preview(string searchId);
        Task<KeyValuePair<bool, string>> UpdateRowCount(string id, int rowCount);
        Task<KeyValuePair<bool, object>> GetStaged(string id, string keyname);
        Task<SearchRestrictionDto> GetSearchRestriction(string userId);
        Task<bool> CreateInvoice(string userId, string searchId);
        Task<IEnumerable<SearchInvoiceBo>> Invoices(string userId, string? searchId);
        Task<PurchaseSummaryDto?> GetPurchaseSummary(string externalId);
        Task<bool> SetInvoicePurchaseDate(string externalId);
        Task<bool> IsValidExternalId(string externalId);
        Task<InvoiceDescriptionDto> InvoiceDescription(string id);
        Task<bool> RequeueSearches();
        Task<IEnumerable<SearchFinalBo>> GetFinal(string searchId);
        Task<ActiveSearchOverviewBo?> GetActiveSearches(string searchId);
        Task<IEnumerable<ActiveSearchDetailBo>?> GetActiveSearchDetails(string userId);
        Task<bool> UpdateSearchRowCount();
        Task<PaymentSessionDto?> GetPaymentSession(string externalId);
        Task<bool> AppendPaymentSession(PaymentSessionDto dto);
        Task<bool?> IsSearchPurchased(string searchId);
        Task<bool?> IsSearchDownloaded(string searchId);
        Task<SearchIsPaidDto?> IsSearchPaidAndDownloaded(string searchId);
        Task<KeyValuePair<bool, string>> CreateOrUpdateDownloadRecord(string searchId, string? content = null);
        Task<IEnumerable<PurchasedSearchBo>?> GetPurchases(string userId);
        Task<DownloadHistoryDto?> AllowDownloadRollback(string userId, string searchId);
        Task<bool> AppendAdHocSession(AdHocSessionBo dto);
        Task<bool> CompleteAdHocSession(string externalId);
        Task<AdHocSessionBo?> FindAdHocSession(string externalId);
        Task<bool> FlagError(string searchId);
        Task<bool> ExtendRestriction(string userId);
    }
}
