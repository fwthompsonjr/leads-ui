namespace legallead.content.interfaces
{
    public interface IWebPageRepository
    {
        IWebContentLineRepository LineRepository { get; }
        IWebContentRepository ContentRepository { get; }
    }
}