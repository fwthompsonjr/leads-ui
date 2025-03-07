﻿using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using System.Diagnostics.CodeAnalysis;

namespace legallead.jdbc.implementations
{
    public class SearchQueueRepository :
        BaseRepository<SearchDto>, ISearchQueueRepository
    {
        private readonly IUserSearchRepository searchrepo;
        public SearchQueueRepository(DataContext context, IUserSearchRepository userSearch) : base(context)
        {
            searchrepo = userSearch;
        }

        public async Task<List<SearchQueueDto>> GetQueue()
        {
            const string prc = "CALL USP_QUERY_USER_SEARCH_QUEUE();";
            using var connection = _context.CreateConnection();
            var response = await _command.QueryAsync<SearchQueueDto>(connection, prc);
            List<SearchQueueDto> list = await ExtractRestrictions(response);
            return list;
        }

        public async Task<SearchQueueDto?> GetQueueItem(string? searchId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchId)) return null;
                if (!Guid.TryParse(searchId, out var _)) return null;
                const string prc = "CALL USP_QUERY_USER_SEARCH_QUEUE_ITEM( ? );";
                var parms = new DynamicParameters();
                parms.Add("search_index", searchId);
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<SearchQueueDto>(connection, prc, parms);
                if (response == null || !response.Any()) return null;
                return response.First();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<KeyValuePair<bool, string>> Complete(string id)
        {
            var response = await searchrepo.Complete(id);
            return response;
        }

        public async Task<KeyValuePair<bool, string>> Start(SearchDto dto)
        {
            var response = await searchrepo.UpdateRowCount(dto.Id, 0);
            return response;
        }

        public async Task<KeyValuePair<bool, string>> Status(string id, string message)
        {
            var response = await searchrepo.Append(SearchTargetTypes.Status, id, message);
            return response;
        }
        public async Task<KeyValuePair<bool, string>> Content(string id, byte[] content)
        {
            var response = await searchrepo.Append(SearchTargetTypes.Response, id, content);
            return response;
        }


        [ExcludeFromCodeCoverage]
        private async Task<List<SearchQueueDto>> ExtractRestrictions(IEnumerable<SearchQueueDto> response)
        {
            var removalids = new List<string>();
            foreach (var item in response)
            {
                var isRestricted = await IsRestricted(item);
                if (isRestricted) { removalids.Add(item.Id); }
            }
            var list = response.ToList();
            list.RemoveAll(x => removalids.Contains(x.Id));
            return list;
        }

        [ExcludeFromCodeCoverage]
        private async Task<bool> IsRestricted(SearchQueueDto queued)
        {
            if (string.IsNullOrEmpty(queued.UserId)) return true;
            var dto = await searchrepo.GetSearchRestriction(queued.UserId);
            if (!dto.MaxPerMonth.HasValue) return true;
            if (dto.IsLocked.GetValueOrDefault()) return true;
            if (dto.ThisMonth.GetValueOrDefault() >= dto.MaxPerMonth.GetValueOrDefault()) return true;
            if (dto.ThisYear.GetValueOrDefault() >= dto.MaxPerYear.GetValueOrDefault()) return true;
            return false;
        }
    }
}