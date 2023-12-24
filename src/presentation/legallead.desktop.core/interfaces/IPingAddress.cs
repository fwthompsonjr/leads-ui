using System.Net.NetworkInformation;

namespace legallead.desktop.interfaces
{
    public interface IPingAddress
    {
        IPStatus CheckStatus(string address);
    }
}