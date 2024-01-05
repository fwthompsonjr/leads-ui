using legallead.permissions.api.Model;

namespace legallead.permissions.api
{
    public interface IStateSearchProvider
    {
        List<StateSearchData> GetStates();
    }
}