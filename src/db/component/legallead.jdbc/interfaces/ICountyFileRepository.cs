using legallead.jdbc.models;

namespace legallead.jdbc.interfaces
{
    public interface ICountyFileRepository
    {
        Task<bool> InitializeAsync();
        Task<DbCountyFileModel?> GetContentAsync(DbCountyFileModel request);
        Task<KeyValuePair<bool, string>> UpdateContentAsync(DbCountyFileModel request);
        Task<KeyValuePair<bool, string>> UpdateTypeAsync(DbCountyFileModel request);
        Task<KeyValuePair<bool, string>> UpdateStatusAsync(DbCountyFileModel request);
    }
}