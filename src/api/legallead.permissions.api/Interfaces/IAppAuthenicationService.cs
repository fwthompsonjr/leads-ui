using legallead.permissions.api.Models;

namespace legallead.permissions.api.Interfaces
{
    public interface IAppAuthenicationService
    {
        AppAuthenicationItemDto? Authenicate(string userName, string password);
        AppAuthenicationItemDto? FindUser(string userName, int id);
    }
}
