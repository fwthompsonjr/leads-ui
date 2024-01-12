using legallead.json.db;
using legallead.json.db.entity;
using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace legallead.permissions.api.Utility
{
    public class UserSearchValidator : IUserSearchValidator
    {
        public UserSearchValidator()
        {
            UsState.Initialize();
            UsStateCounty.Initialize();
        }
        public int MaxDays { get; set; }
        public long MinStartDate { get; set; }

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
            return CheckParameters(findCounty, request.Details);
        }

        private static DateTime ToDateTime(long time)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(time).DateTime;
        }

        [ExcludeFromCodeCoverage(Justification ="Private member tested from public exposed method.")]
        private static KeyValuePair<bool, string> CheckParameters(UsStateCounty findCounty, List<CountyParameterModel> details)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            var success = new KeyValuePair<bool, string>(true, "Model is valid");
            var state = (findCounty.StateCode ?? string.Empty).ToLower();
            var county = (findCounty.Name ?? string.Empty).Replace(" ", "-").ToLower();
            var keyname = $"{state}-{county}-drop-down";
            var property = TryGetResource(keyname);
            if (property == null)
            {
                return new KeyValuePair<bool, string>(false, "Model parameters are invalid.");
            }
            var expectedCount = property.DropDowns
                    .Count(w => w.IsDisplayed.GetValueOrDefault());
            if (property.CaseSearchTypes != null) { expectedCount++; }
            if (details.Count != expectedCount)
            {
                return new KeyValuePair<bool, string>(false, "Model parameters count is not invalid.");
            }
            var members = property.DropDowns.SelectMany(m => m.Members).ToList();
            var dropdowns = details.FindAll(x => x.Name != "Case Type");
            foreach ( var dropdown in dropdowns )
            {
                var failedDropDown = new KeyValuePair<bool, string>(false, $"Parameter invalid for {dropdown.Name}");
                if (!int.TryParse(dropdown.Value, out var ddIndex)) return failedDropDown;
                var found = members.Find(x => x.Name.Equals(dropdown.Text, oic) && x.Id.Equals(ddIndex));
                if (found == null) return failedDropDown;
            }
            if (property.CaseSearchTypes == null) return success;
            var caseselection = details.Find(x => x.Name == "Case Type");
            var failedCase = new KeyValuePair<bool, string>(false, $"Parameter invalid for case type");
            var caseid = caseselection == null ? -1 : Convert.ToInt32(caseselection.Value);
            if (caseselection == null) return failedCase;
            var isfound = property.CaseSearchTypes.Exists(a => a.Name.Equals(caseselection.Text, oic) && a.Id == caseid);
            if (!isfound) return failedCase;
            return success;
        }
        
        [ExcludeFromCodeCoverage(Justification = "Private member tested from public exposed method.")]
        private static CountySearchDetail? TryGetResource(string name)
        {
            try
            {
                var manager = Properties.Resources.ResourceManager;
                var itemName = name.Replace("-", "_");
                var content = manager.GetString(itemName);
                if (string.IsNullOrWhiteSpace(content)) { return null; }
                return JsonConvert.DeserializeObject<CountySearchDetail>(content);
            } catch
            {
                return null;
            }
        }

    }
}
