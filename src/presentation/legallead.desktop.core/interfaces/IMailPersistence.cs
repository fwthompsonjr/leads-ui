namespace legallead.desktop.interfaces
{
    internal interface IMailPersistence
    {
        void Clear();
        void Save(string json);
        string? Fetch();
        void Save(string id, string json);
        string? Fetch(string id);
    }
}
