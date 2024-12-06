using legallead.jdbc.interfaces;
using legallead.permissions.api.Models;

namespace legallead.permissions.api.Services
{
    public class DbHistoryService(IDbHistoryRepository db) : IDbHistoryService
    {
        private readonly IDbHistoryRepository _db = db;

        public Task<DataRequestResponse> BeginAsync(BeginDataRequest model)
        {
            throw new NotImplementedException();
        }

        public Task<DataRequestResponse> CompleteAsync(CompleteDataRequest model)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FindRequestResponse>> FindAsync(FindDataRequest model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UploadAsync(UploadDataRequest model)
        {
            throw new NotImplementedException();
        }
    }
}
