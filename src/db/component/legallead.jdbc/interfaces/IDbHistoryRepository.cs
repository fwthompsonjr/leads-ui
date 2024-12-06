using legallead.jdbc.entities;
using legallead.jdbc.models;

namespace legallead.jdbc.interfaces
{
    public interface IDbHistoryRepository
    {
        Task<DbSearchHistoryBo?> BeginAsync(DbHistoryRequest request);
        Task<DbSearchHistoryBo?> CompleteAsync(DbHistoryRequest request);
        Task<List<DbSearchHistoryResultBo>?> FindAsync(string id);
        Task<bool> UploadAsync(DbUploadRequest request);
    }
}
