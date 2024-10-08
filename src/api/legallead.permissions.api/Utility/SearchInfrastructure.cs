﻿using legallead.jdbc;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Models;
using Newtonsoft.Json;
using System.Globalization;

namespace legallead.permissions.api.Utility
{
    [ExcludeFromCodeCoverage(Justification = "Coverage is to be handled later. Reference GitHub Issue")]
    public class SearchInfrastructure : ISearchInfrastructure
    {
        protected readonly IDataProvider _db;
        private readonly IUserSearchRepository _repo;
        private readonly IRequestedUser _usrDb;
        internal SearchInfrastructure(IDataProvider db, IUserSearchRepository repo, IRequestedUser usr)
        {
            _db = db;
            _repo = repo;
            _usrDb = usr;
        }

        public async Task<SearchRestrictionModel?> GetRestrictionStatusAsync(HttpRequest http)
        {
            var user = await GetUserAsync(http);
            if (user == null) return null;
            var restrictions = await _repo.GetSearchRestriction(user.Id) ?? new();
            return restrictions.ToModel();
        }

        public async Task<bool?> ExtendRestrictionAsync(HttpRequest http)
        {
            var user = await GetUserAsync(http);
            if (user == null) return null;
            var isSubmitted = await _repo.ExtendRestriction(user.Id);
            return isSubmitted;
        }

        public async Task<UserSearchBeginResponse?> BeginAsync(HttpRequest http, UserSearchRequest request)
        {
            var user = await GetUserAsync(http);
            if (user == null) return null;
            var restrictions = await _repo.GetSearchRestriction(user.Id) ?? new();
            if (restrictions.IsRestricted())
            {
                return restrictions.GetRestrictionResponse();
            }
            var searchRecord = await _repo.Begin(user.Id, JsonConvert.SerializeObject(request));
            if (searchRecord.Key)
            {
                await _repo.Append(SearchTargetTypes.Status, searchRecord.Value, "INFO: Search request submitted.");
            }
            return new()
            {
                Request = request,
                RequestId = searchRecord.Key ? searchRecord.Value : string.Empty
            };
        }

        public async Task<IEnumerable<SearchPreviewBo>?> GetPreviewAsync(HttpRequest http, string searchId)
        {
            var user = await GetUserAsync(http);
            if (user == null) return null;
            var restrictions = await _repo.GetSearchRestriction(user.Id) ?? new();
            if (restrictions.IsRestricted())
            {
                return null;
            }
            var searches = await GetHeaderAsync(http, searchId);
            if (searches == null || !searches.Any()) return Array.Empty<SearchPreviewBo>();
            var rawdata = await _repo.Preview(searchId);
            if (rawdata == null || !rawdata.Any()) return null;
            // apply limit to max allowed per month/year
            var filtered = SearchRestrictionFilter.FilterByRestriction(rawdata, restrictions);
            return SearchRestrictionFilter.Sanitize(filtered);
        }

        public async Task<IEnumerable<UserSearchQueryModel>?> GetHeaderAsync(HttpRequest http, string? id)
        {
            var user = await GetUserAsync(http);
            if (user == null) return null;
            _ = await _repo.UpdateSearchRowCount();
            var histories = await _repo.History(user.Id);
            if (histories == null || !histories.Any()) return Enumerable.Empty<UserSearchQueryModel>();
            if (!string.IsNullOrEmpty(id))
            {
                histories = histories.Where(h => (h.Id ?? "-").Equals(id, StringComparison.OrdinalIgnoreCase));
            }
            var models = histories.Select(x => new UserSearchQueryModel
            {
                Id = x.Id ?? string.Empty,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                EstimatedRowCount = x.EstimatedRowCount,
                CreateDate = x.CreateDate,
                SearchProgress = x.SearchProgress,
                StateCode = x.StateCode,
                CountyName = x.CountyName

            });
            return models;
        }

        public async Task<IEnumerable<UserSearchDetail>?> GetDetailAsync(HttpRequest http, string? id)
        {
            return await GetDataAsync(http, SearchTargetTypes.Detail, id);
        }

        public async Task<IEnumerable<UserSearchDetail>?> GetResultAsync(HttpRequest http, string? id)
        {
            return await GetDataAsync(http, SearchTargetTypes.Response, id);
        }

        public async Task<IEnumerable<UserSearchDetail>?> GetStatusAsync(HttpRequest http, string? id)
        {
            return await GetDataAsync(http, SearchTargetTypes.Status, id);
        }

        public async Task<bool> SetDetailAsync(string id, object detail)
        {
            var payload = JsonConvert.SerializeObject(detail) ?? string.Empty;
            var response = await SetDataAsync(SearchTargetTypes.Detail, id, payload);
            return response.Key;
        }

        public async Task<bool> SetResultAsync(string id, object result)
        {
            var payload = JsonConvert.SerializeObject(result) ?? string.Empty;
            var response = await SetDataAsync(SearchTargetTypes.Response, id, payload);
            return response.Key;
        }

        public async Task<bool> SetStatusAsync(string id, string message)
        {
            var response = await SetDataAsync(SearchTargetTypes.Status, id, message);
            return response.Key;
        }
        public async Task<bool> SetResponseAsync(string id, object response)
        {
            var payload = JsonConvert.SerializeObject(response) ?? string.Empty;
            var data = await SetDataAsync(SearchTargetTypes.Response, id, payload);
            return data.Key;
        }

        public async Task<User?> GetUserAsync(HttpRequest request)
        {
            return await _usrDb.GetUserAsync(request);
        }

        public async Task<IEnumerable<SearchInvoiceBo>?> CreateInvoiceAsync(string userid, string searchid)
        {
            var hasinvoice = await _repo.CreateInvoice(userid, searchid);
            if (!hasinvoice) { return null; }
            var records = await _repo.Invoices(userid, searchid);
            return records;
        }

        public async Task<IEnumerable<SearchInvoiceBo>?> GetInvoicesAsync(string userid, string? searchid)
        {
            var records = await _repo.Invoices(userid, searchid);
            return records;
        }

        public async Task<ActiveSearchOverviewBo?> GetSearchProgressAsync(string searchId)
        {
            var history = await _repo.GetActiveSearches(searchId);
            return history;
        }
        public async Task<object?> GetSearchDetailsAsync(string userId)
        {
            var searches = await _repo.GetActiveSearchDetails(userId);
            if (searches == null || !searches.Any()) return new();
            var tempid = searches.ToList()[^1].Id ?? string.Empty;
            var history = await _repo.GetActiveSearches(tempid);
            if (history != null && history.Staged != null && history.Staged.Any())
            {
                history.Staged = FormatWeb(history.Staged);
            }
            return new
            {
                details = searches,
                history
            };
        }

        public async Task<IEnumerable<PurchasedSearchBo>?> GetPurchasesAsync(string userId)
        {
            var searches = await _repo.GetPurchases(userId);
            return searches;
        }

        public async Task<bool> FlagErrorAsync(string searchId)
        {
            var tmp = await _repo.FlagError(searchId);
            return tmp;
        }
        private static IEnumerable<SearchStagingSummaryBo> FormatWeb(IEnumerable<SearchStagingSummaryBo> staged)
        {
            const string beginWith = "data-";
            const string find = "data-output-person-addres";
            const string replace = "data-output-person-address";
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            var misaddressed = staged.Where(w => (w.StagingType ?? string.Empty).Equals(find)).ToList();
            misaddressed.ForEach(m => m.StagingType = replace);
            var data = staged.Where(w => (w.StagingType ?? string.Empty).StartsWith(beginWith)).ToList();
            data.ForEach(m =>
            {
                var words = (m.StagingType ?? string.Empty).Split('-');
                var phrase = string.Join(" ", words).ToLower();
                m.StagingType = textInfo.ToTitleCase(phrase);
            });
            return staged;
        }

        private async Task<IEnumerable<UserSearchDetail>?> GetDataAsync(HttpRequest http, SearchTargetTypes target, string? id)
        {
            var user = await GetUserAsync(http);
            if (user == null) return null;
            var records = await _repo.GetTargets(target, user.Id, id);
            if (records == null || !records.Any()) return Enumerable.Empty<UserSearchDetail>();
            var models = records.Select(x => new UserSearchDetail
            {
                Id = x.SearchId ?? string.Empty,
                Context = x.Component ?? target.ToString().ToUpper(),
                LineNumber = x.LineNbr.GetValueOrDefault(),
                Line = x.Line ?? string.Empty,
                CreateDate = x.CreateDate.GetValueOrDefault(),
            });
            return models;
        }


        private async Task<KeyValuePair<bool, string>> SetDataAsync(SearchTargetTypes target, string id, string message)
        {
            var records = await _repo.Append(target, id, message);
            return records;
        }
    }
}
