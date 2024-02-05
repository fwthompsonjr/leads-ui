using legallead.permissions.api.Model;

namespace legallead.permissions.api.Interfaces
{
    public interface IStateSearchProvider
    {
        List<StateSearchData> GetStates();
    }
}