using legallead.records.search.Models;

namespace legallead.records.search.Interfaces
{
    public interface IStagingPersistence
    {
        KeyValuePair<bool, string> Add(string searchid, string key, string value);
        KeyValuePair<bool, string> Add(string searchid, string key, byte[] value);
        Task<KeyValuePair<bool, string>> AddAsync(string searchid, string key, string value);
        Task<KeyValuePair<bool, string>> AddAsync(string searchid, string key, byte[] value);

        KeyValuePair<bool, StagedContentModel> Fetch(string searchid, string key);
        Task<KeyValuePair<bool, StagedContentModel>> FetchAsync(string searchid, string key);
    }
}
