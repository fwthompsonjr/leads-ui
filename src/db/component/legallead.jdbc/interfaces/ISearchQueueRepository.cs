using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface ISearchQueueRepository
    {
        Task<List<SearchQueueDto>> GetQueue();
        Task<SearchQueueDto?> GetQueueItem(string? searchId);
        Task<KeyValuePair<bool, string>> Start(SearchDto dto);
        Task<KeyValuePair<bool, string>> Status(string id, string message);
        Task<KeyValuePair<bool, string>> Content(string id, byte[] content);
        Task<KeyValuePair<bool, string>> Complete(string id);
    }
}
