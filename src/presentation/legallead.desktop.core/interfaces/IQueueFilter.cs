namespace legallead.desktop.interfaces
{
    public interface IQueueFilter
    {
        void Append(string userId);
        void Clear();
    }
}
