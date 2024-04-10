using legallead.models;

namespace legallead.permissions.api.Interfaces
{
    public interface IStateSearchProvider
    {
        List<StateSearchData> GetStates();
    }
}