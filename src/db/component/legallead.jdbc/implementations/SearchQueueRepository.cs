﻿using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using System.Text;

namespace legallead.jdbc.implementations
{
    public class SearchQueueRepository : 
        BaseRepository<SearchDto>, ISearchQueueRepository
    {
        private readonly IUserSearchRepository searchrepo;
        public SearchQueueRepository(DataContext context) : base(context)
        {
            searchrepo = new UserSearchRepository(context);
        }

        public async Task<List<SearchDto>> GetQueue()
        {
            const string prc = "CALL USP_QUERY_USER_SEARCH_QUEUE();";
            using var connection = _context.CreateConnection();
            var response = await _command.QueryAsync<SearchDto>(connection, prc);
            return response.ToList();
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
    }
}