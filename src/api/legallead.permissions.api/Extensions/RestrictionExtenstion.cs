using legallead.permissions.api.Models;
using Newtonsoft.Json;

namespace legallead.permissions.api.Extensions
{
    public static class RestrictionExtenstion
    {
        public static SearchRestrictionModel? ToModel(this SearchRestrictionDto dto)
        {
            var serial = JsonConvert.SerializeObject(dto);
            var model = JsonConvert.DeserializeObject<SearchRestrictionModel>(serial);
            if (model == null) return null;
            model.Reason = GetRestrictionMessage(model);
            model.IsLocked = !string.IsNullOrEmpty(model.Reason);
            return model;
        }

        private static string GetRestrictionMessage(SearchRestrictionModel model)
        {
            if (model.ThisMonth.GetValueOrDefault() >= model.MaxPerMonth.GetValueOrDefault())
            {
                var mtd = $"Month to date: {model.ThisMonth.GetValueOrDefault()}. Limit: {model.MaxPerMonth.GetValueOrDefault()}";
                return $"Monthly Limit Exceeded - {mtd}";
            }
            if (model.ThisYear.GetValueOrDefault() >= model.MaxPerYear.GetValueOrDefault())
            {
                var ytd = $"Year to date: {model.ThisYear.GetValueOrDefault()}. Limit: {model.MaxPerYear.GetValueOrDefault()}";
                return $"Annual Limit Exceeded - {ytd}";
            }
            return string.Empty;
        }
    }
}
