namespace legallead.permissions.api.Interfaces
{
    public interface IStartupTask
    {
        int Index { get; }

        Task ExecuteAsync();
    }
}