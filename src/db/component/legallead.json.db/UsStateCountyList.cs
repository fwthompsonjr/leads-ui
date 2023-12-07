using legallead.json.db.entity;

namespace legallead.json.db
{
    internal static class UsStateCountyList
    {
        private static bool IsPopulated;
        private static readonly List<UsStateCounty> list = new();

        public static void Populate(List<UsStateCounty> items)
        {
            if (IsPopulated) return;
            list.AddRange(items);
            IsPopulated = true;
        }

        public static bool Verify(string? county)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            if (string.IsNullOrEmpty(county)) return false;
            if (!IsPopulated || list.Count == 0) return true;
            var item = list.Find(x => x.IsActive && (x.Name ?? "").Equals(county, oic) || (x.ShortName ?? "").Equals(county, oic));
            return item != null;
        }

        public static UsStateCounty? Find(string? county)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;

            if (string.IsNullOrEmpty(county)) return null;
            if (!IsPopulated || list.Count == 0) return null;
            return list.Find(x => x.IsActive && (x.Name ?? "").Equals(county, oic) || (x.ShortName ?? "").Equals(county, oic));
        }

        public static List<UsStateCounty>? FindAll(string? county)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;

            if (string.IsNullOrEmpty(county)) return null;
            if (!IsPopulated || list.Count == 0) return null;
            var found = list.FindAll(x => x.IsActive && (x.Name ?? "").Equals(county, oic) || (x.ShortName ?? "").Equals(county, oic));
            if (found == null || !found.Any()) return null;
            return found;
        }
    }
}