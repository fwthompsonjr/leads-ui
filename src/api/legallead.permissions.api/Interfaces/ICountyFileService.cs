namespace legallead.permissions.api.Interfaces
{
    public interface ICountyFileService
    {
        Task<KeyValuePair<bool, string>> SaveAsync(DbCountyFileModel model);
        Task<DbCountyFileModel?> GetAsync(DbCountyFileModel model);
    }
}