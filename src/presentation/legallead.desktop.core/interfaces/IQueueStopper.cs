namespace legallead.desktop.interfaces
{
    public interface IQueueStopper
    {
        string ServiceName { get; }
        void Stop();
    }
}
