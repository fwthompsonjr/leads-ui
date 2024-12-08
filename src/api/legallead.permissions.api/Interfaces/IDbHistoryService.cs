using legallead.permissions.api.Models;

namespace legallead.permissions.api.Interfaces
{
    public interface IDbHistoryService
    {
        Task<DataRequestResponse?> BeginAsync(BeginDataRequest model);
        Task<DataRequestResponse?> CompleteAsync(CompleteDataRequest model);
        Task<IEnumerable<FindRequestResponse>?> FindAsync(FindDataRequest model);
        Task<KeyValuePair<bool, string>> UploadAsync(UploadDataRequest model);
    }
}
