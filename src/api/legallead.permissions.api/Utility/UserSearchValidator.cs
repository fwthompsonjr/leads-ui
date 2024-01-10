using legallead.json.db;
using legallead.json.db.entity;
using legallead.permissions.api.Model;

namespace legallead.permissions.api.Utility
{
    public class UserSearchValidator
    {
        public UserSearchValidator()
        {
            UsState.Initialize();
            UsStateCounty.Initialize();
        }
        public int MaxDays { get; set; }
        public long MinStartDate { get; set; }
        public string Api { get; set; } = string.Empty;
        public KeyValuePair<bool, string> IsValid(UserSearchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.State))
            {
                return new KeyValuePair<bool, string>(false, "State parameter is invalid.");
            }
            var state = UsStatesList.Find(request.State);
            if (state == null || !state.IsActive)
            {
                return new KeyValuePair<bool, string>(false, $"State parameter: {request.State} not a valid search.");
            }
            var county = request.County;
            if (string.IsNullOrWhiteSpace(county.Name) || county.Value == 0)
            {
                return new KeyValuePair<bool, string>(false, "County parameter is invalid.");
            }
            var findCounty = UsStateCountyList.Find(county.Name);
            if (findCounty == null || !findCounty.IsActive || findCounty.Index != county.Value)
            {
                return new KeyValuePair<bool, string>(false, $"County parameter: {county.Value} - {county.Name} not a valid search.");
            }
            if (request.StartDate < MinStartDate)
            {
                return new KeyValuePair<bool, string>(false, $"Start Date parameter is prior to minimum date, 1/1/2018.");
            }
            var startDate = ToDateTime(request.StartDate);
            var endDate = ToDateTime(request.EndDate);
            var difference = endDate.Subtract(startDate).TotalDays;
            if (difference > MaxDays)
            {
                return new KeyValuePair<bool, string>(false, $"Date range must be ({MaxDays}) or less.");
            }
            return new KeyValuePair<bool, string>(true, "Model is valid");
        }

        private static DateTime ToDateTime(long time)
        {

            return DateTimeOffset.FromUnixTimeMilliseconds(time).DateTime;
        }
    }
}
