using legallead.json.db.entity;

namespace legallead.permissions.api.Model
{
    public class StateSearchData : UsState
    {
        public List<CountySearchData>? Counties { get; set; }
    }
}