using legallead.desktop.entities;
using Newtonsoft.Json;
using System;

namespace legallead.desktop.utilities
{
    internal static class JsSearchSubmissionHelper
    {
        public static object Refine(object payload)
        {
            const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            if (payload is not BeginSearchModel model) return payload;
            if (!model.County.Name.Equals("Harris", comparison)) return model;
            model.Details.Clear();
            model.Details.Add(new() { Name = "Search Type", Text = "All Civil Courts", Value = "0" });
            return model;
        }

        public static string SortHistory(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return string.Empty;
            var model = ObjectExtensions.TryGet<MyActiveSearch>(json);
            if (model.Details.Count == 0) return json;
            model.Details.Sort((a,b) => b.CreateDate.CompareTo(a.CreateDate));
            return JsonConvert.SerializeObject(model);
        }
    }
}
