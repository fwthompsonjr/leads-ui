using legallead.jdbc.entities;
using legallead.jdbc.models;

namespace legallead.jdbc.interfaces
{
    internal interface IUserSearchRepository
    {
        Task<KeyValuePair<bool, string>> Append(SearchTargetTypes search, string? id, object data);
        Task<KeyValuePair<bool, string>> Begin(string userId, string payload);
        Task<KeyValuePair<bool, string>> Complete(string id);
        Task<IEnumerable<SearchTargetModel>?> GetTargets(SearchTargetTypes search, string? userId, string? id);
        Task<IEnumerable<SearchDtoHeader>> History(string userId);
        Task<KeyValuePair<bool, string>> UpdateRowCount(string id, int rowCount);
    }
}
