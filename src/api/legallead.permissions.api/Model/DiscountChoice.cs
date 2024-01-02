using legallead.json.db;
using legallead.json.db.entity;

namespace legallead.permissions.api.Model
{
    public class DiscountChoice
    {
        public string StateName { get; set; } = string.Empty;
        public string CountyName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }

        internal UsState? ToState()
        {
            if (string.IsNullOrEmpty(StateName)) return null;
            return UsStatesList.Find(StateName);
        }

        internal UsStateCounty? ToCounty()
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            var state = ToState();
            if (state == null) return null;
            if (string.IsNullOrEmpty(CountyName)) return null;
            var counties = UsStateCountyList.FindAll(CountyName);
            if (counties == null) return null;
            return counties.Find(x =>
            {
                var stateCode = x.StateCode ?? string.Empty;
                if (stateCode.Equals(state.Name, oic)) return true;
                if (stateCode.Equals(state.ShortName, oic)) return true;
                return false;
            });
        }
    }
}