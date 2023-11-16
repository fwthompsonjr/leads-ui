using legallead.harriscriminal.db.Entities;

namespace legallead.harriscriminal.db.Interfaces
{
    public interface IDataLoad<T> where T : class
    {
        List<string> FileNames { get; }

        List<T> Load(bool reset = false);

        Task<List<T>> LoadAsyc(IProgress<DataLoadDto> progress, bool reset = false);
    }
}