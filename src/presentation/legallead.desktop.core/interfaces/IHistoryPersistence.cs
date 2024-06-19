namespace legallead.desktop.interfaces
{
    internal interface IHistoryPersistence
    {
        void Clear();
        void Save(string json);
        string? Fetch();
    }
}