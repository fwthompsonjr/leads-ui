using legallead.jdbc.models;

namespace legallead.jdbc.interfaces
{
    internal interface IUserSearchRepository
    {
        Task<KeyValuePair<bool, string>> Append(SearchTargetTypes search, string? id, string data);
        Task<KeyValuePair<bool, string>> Begin(string userId, string payload);
        Task<KeyValuePair<bool, string>> Complete(string id);
        Task<IEnumerable<SearchTargetModel>> GetTargets(SearchTargetTypes search, string? id);
    }
}
