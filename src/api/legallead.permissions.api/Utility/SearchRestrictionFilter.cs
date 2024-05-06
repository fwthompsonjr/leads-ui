namespace legallead.permissions.api.Utility
{
    internal static class SearchRestrictionFilter
    {
        public static IEnumerable<SearchPreviewBo> FilterByRestriction(IEnumerable<SearchPreviewBo> source, SearchRestrictionDto dto)
        {
            var count = source.Count();
            var mxpermonth = dto.MaxPerMonth.GetValueOrDefault(count) - dto.ThisMonth.GetValueOrDefault();
            var mxperyear = dto.MaxPerYear.GetValueOrDefault(count) - dto.ThisYear.GetValueOrDefault();
            var allowed = Math.Min(mxpermonth, mxperyear);
            if (count <= allowed) { return source; }
            return source.Take(allowed);
        }

        public static IEnumerable<SearchPreviewBo> Sanitize(IEnumerable<SearchPreviewBo> source)
        {
            var list = source.ToList();
            for (var i = 0; i < list.Count; i++)
            {
                list[i] = Sanitize(list[i]);
            }
            return list;
        }


        private static SearchPreviewBo Sanitize(SearchPreviewBo source)
        {
            const string question = "?";
            var response = new SearchPreviewBo
            {
                Name = ResetDots($"{Truncate(source.Name ?? question, 2)}:{TruncateZip(source.Zip)}"),
                CaseNumber = Truncate(source.CaseNumber ?? question, 8),
                Court = Truncate(source.Court ?? question, 15),
                DateFiled = source.DateFiled ?? question,
                CaseType = Truncate(source.CaseType ?? question, 15)
            };
            return response;
        }
        private static string Truncate(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value[..maxChars] + "...";
        }
        private static string TruncateZip(string? zip)
        {
            const string nozip = "0000x";
            if (string.IsNullOrEmpty(zip)) return nozip;
            if (zip.Length < 4) return nozip;
            return string.Concat(zip[..4], "x");
        }
        private static string ResetDots(string source)
        {
            const string dots = "...";
            if (string.IsNullOrEmpty(source)) return source;
            if (!source.Contains(dots)) return source;
            return source.Replace(dots, string.Empty);
        }
    }
}
