
using legallead.jdbc.interfaces;

namespace legallead.permissions.api.Services
{
    public class CountyFileService(ICountyFileRepository db) : ICountyFileService
    {
        private readonly ICountyFileRepository _db = db;
        public async Task<KeyValuePair<bool, string>> SaveAsync(DbCountyFileModel model)
        {
            try
            {
                var updateTasks = new List<Task<KeyValuePair<bool, string>>>
                {
                    _db.UpdateTypeAsync(model),
                    _db.UpdateStatusAsync(model),
                    _db.UpdateContentAsync(model)
                };

                var results = await Task.WhenAll(updateTasks);

                var failure = results.FirstOrDefault(r => !r.Key);
                if (!failure.Key && !string.IsNullOrWhiteSpace(failure.Value)) return failure;

                return new KeyValuePair<bool, string>(true, "");
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }


        public async Task<DbCountyFileModel?> GetAsync(DbCountyFileModel model)
        {
            try
            {
                var result = await _db.GetContentAsync(model);
                return result;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}