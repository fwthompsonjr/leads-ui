using legallead.permissions.api.Model;

namespace legallead.permissions.api.Utility
{
    public class StateSearchProvider : IStateSearchProvider
    {
        public List<StateSearchData> GetStates()
        {
            return new();
        }
    }
}