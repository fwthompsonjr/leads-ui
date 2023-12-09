namespace legallead.permissions.api
{
    public interface IStartupTask
    {
        int Index { get; }
        Task Execute();
    }
}