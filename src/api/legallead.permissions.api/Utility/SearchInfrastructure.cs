using legallead.jdbc;
using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Model;
using legallead.permissions.api.Models;
using Newtonsoft.Json;

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

        public async Task<IEnumerable<UserSearchQueryModel>?> GetHeader(HttpRequest http, string? id)
        {
            var user = await GetUser(http);
            if (user == null) return null;
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
