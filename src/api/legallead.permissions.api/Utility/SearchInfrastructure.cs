using legallead.jdbc;
using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Model;
using legallead.permissions.api.Models;
using Newtonsoft.Json;
using System.Globalization;

namespace legallead.permissions.api.Utility
{
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

        public async Task<UserSearchBeginResponse?> Begin(HttpRequest http, UserSearchRequest request)
        {
            var user = await GetUser(http);
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

        public async Task<IEnumerable<SearchPreviewBo>?> GetPreview(HttpRequest http, string searchId)
        {
            var user = await GetUser(http);
            if (user == null) return null;
            var restrictions = await _repo.GetSearchRestriction(user.Id) ?? new();
            if (restrictions.IsRestricted())
            {
                return null;
            }
            var searches = await GetHeader(http, searchId);
            if (searches == null || !searches.Any()) return Array.Empty<SearchPreviewBo>();
            var rawdata = await _repo.Preview(searchId);
            if (rawdata == null || !rawdata.Any()) return null;
            // apply limit to max allowed per month/year
            var filtered = SearchRestrictionFilter.FilterByRestriction(rawdata, restrictions);
            return SearchRestrictionFilter.Sanitize(filtered);
        }

        public async Task<IEnumerable<UserSearchQueryModel>?> GetHeader(HttpRequest http, string? id)
        {
            var user = await GetUser(http);
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

        public async Task<IEnumerable<UserSearchDetail>?> GetDetail(HttpRequest http, string? id)
        {
            return await GetData(http, SearchTargetTypes.Detail, id);
        }

        public async Task<IEnumerable<UserSearchDetail>?> GetResult(HttpRequest http, string? id)
        {
            return await GetData(http, SearchTargetTypes.Response, id);
        }

        public async Task<IEnumerable<UserSearchDetail>?> GetStatus(HttpRequest http, string? id)
        {
            return await GetData(http, SearchTargetTypes.Status, id);
        }

        public async Task<bool> SetDetail(string id, object detail)
        {
            var payload = JsonConvert.SerializeObject(detail) ?? string.Empty;
            var response = await SetData(SearchTargetTypes.Detail, id, payload);
            return response.Key;
        }

        public async Task<bool> SetResult(string id, object result)
        {
            var payload = JsonConvert.SerializeObject(result) ?? string.Empty;
            var response = await SetData(SearchTargetTypes.Response, id, payload);
            return response.Key;
        }

        public async Task<bool> SetStatus(string id, string message)
        {
            var response = await SetData(SearchTargetTypes.Status, id, message);
            return response.Key;
        }
        public async Task<bool> SetResponse(string id, object response)
        {
            var payload = JsonConvert.SerializeObject(response) ?? string.Empty;
            var data = await SetData(SearchTargetTypes.Response, id, payload);
            return data.Key;
        }

        public async Task<User?> GetUser(HttpRequest request)
        {
            return await _usrDb.GetUser(request);
        }

        public async Task<IEnumerable<SearchInvoiceBo>?> CreateInvoice(string userid, string searchid)
        {
            var hasinvoice = await _repo.CreateInvoice(userid, searchid);
            if (!hasinvoice) { return null; }
            var records = await _repo.Invoices(userid, searchid);
            return records;
        }

        public async Task<IEnumerable<SearchInvoiceBo>?> GetInvoices(string userid, string? searchid)
        {
            var records = await _repo.Invoices(userid, searchid);
            return records;
        }

        public async Task<ActiveSearchOverviewBo?> GetSearchProgress(string searchId)
        {
            var history = await _repo.GetActiveSearches(searchId);
            return history;
        }
        public async Task<object?> GetSearchDetails(string userId)
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

        public async Task<IEnumerable<PurchasedSearchBo>?> GetPurchases(string userId)
        {
            var searches = await _repo.GetPurchases(userId);
            return searches;
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

        private async Task<IEnumerable<UserSearchDetail>?> GetData(HttpRequest http, SearchTargetTypes target, string? id)
        {
            var user = await GetUser(http);
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


        private async Task<KeyValuePair<bool, string>> SetData(SearchTargetTypes target, string id, string message)
        {
            var records = await _repo.Append(target, id, message);
            return records;
        }
    }
}
