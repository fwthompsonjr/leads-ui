using legallead.models;

namespace legallead.permissions.api.Extensions
{
    using UsState = json.db.entity.UsState;
    using UsStateCounty = json.db.entity.UsStateCounty;

    public static class StateCountyExtensions
    {
        public static UsState? ToState(this DiscountChoice choice)
        {
            var stateName = choice.StateName;
            if (string.IsNullOrEmpty(stateName))
            {
                return null;
            }
            return json.db.UsStatesList.Find(stateName);
        }

        [ExcludeFromCodeCoverage(Justification = "This method is tested through integration tests.")]
        public static UsStateCounty? ToCounty(this DiscountChoice choice)
        {
            UsState? state = choice.ToState();
            if (state == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(choice.CountyName))
            {
                return null;
            }

            return json.db.UsStateCountyList.FindAll(choice.CountyName)?.Find(delegate (UsStateCounty x)
            {
                string text = x.StateCode ?? string.Empty;
                if (text.Equals(state.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                return text.Equals(state.ShortName, StringComparison.OrdinalIgnoreCase);
            });
        }
    }
}
