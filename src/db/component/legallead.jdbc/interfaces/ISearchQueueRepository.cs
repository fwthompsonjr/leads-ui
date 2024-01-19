using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface ISearchQueueRepository
    {
        Task<List<SearchDto>> GetQueue();
        Task<KeyValuePair<bool, string>> Start(SearchDto dto);
        Task<KeyValuePair<bool, string>> Status(string id, string message);
        Task<KeyValuePair<bool, string>> Content(string id, byte[] content);
        Task<KeyValuePair<bool, string>> Complete(string id);
    }
}
