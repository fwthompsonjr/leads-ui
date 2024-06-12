namespace legallead.desktop.interfaces
{
    internal interface IMailPersistence
    {
        void Clear();
        void Save(string json);
        string? Fetch();
    }
}
