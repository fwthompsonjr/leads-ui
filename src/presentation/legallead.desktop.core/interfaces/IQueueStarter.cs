namespace legallead.desktop.interfaces
{
    public interface IQueueStarter
    {
        string ServiceName { get; }
        void Start();
    }
}