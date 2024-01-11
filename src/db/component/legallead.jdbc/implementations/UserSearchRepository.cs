using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.jdbc.implementations
{
    public class UserSearchRepository : BaseRepository<SearchDto>, IUserSearchRepository
    {
        public UserSearchRepository(DataContext context) : base(context)
        {
        }

        public Task<KeyValuePair<bool, string>> Append(SearchTargetTypes search, string? id, string data)
        {
            throw new NotImplementedException();
        }

        public Task<KeyValuePair<bool, string>> Begin(string userId, string payload)
        {
            throw new NotImplementedException();
        }

        public Task<KeyValuePair<bool, string>> Complete(string id)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<SearchTargetModel>> IUserSearchRepository.GetTargets(SearchTargetTypes search, string? id)
        {
            throw new NotImplementedException();
        }
    }
}
