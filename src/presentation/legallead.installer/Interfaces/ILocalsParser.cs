namespace legallead.installer.Interfaces
{
    public interface ILocalsParser
    {
        string GetLatest(string installed, string appName);
    }
}
