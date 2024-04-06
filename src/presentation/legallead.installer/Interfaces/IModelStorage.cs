
namespace legallead.installer.Interfaces
{
    public interface IModelStorage<T> where T : class
    {
        DateTime CreationDate { get; set; }
        List<T> Detail { get; set; }
        bool IsValid { get; }
        string Name { get; }

        List<T>? Find();
        void Save();
    }
}