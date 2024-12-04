using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IHarrisLoadRepository
    {
        Task<KeyValuePair<bool, string>> Append(string data);
        Task<List<HarrisCriminalUploadBo>> Find(DateTime dte);
    }
}