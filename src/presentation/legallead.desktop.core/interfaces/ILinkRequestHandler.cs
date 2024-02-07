using legallead.desktop.entities;

namespace legallead.desktop.interfaces
{
    public interface ILinkRequestHandler
    {
        List<KeyNameBo> GetParameters(string url);
        T GetContent<T>(string url);
    }
}
