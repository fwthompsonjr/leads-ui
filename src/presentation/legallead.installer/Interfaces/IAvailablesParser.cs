namespace legallead.installer.Interfaces
{
    public interface IAvailablesParser
    {
        string GetLatest(string available, string appName);
    }
}