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

        public static bool Verify(string? state)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            if (!IsPopulated || list.Count == 0) return true;
            var item = list.Find(x => x.IsActive && (x.Name ?? "").Equals(state, oic) || (x.ShortName ?? "").Equals(state, oic));
            return item != null;
        }

        public static UsStateCounty? Find(string? state)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            if (!IsPopulated || list.Count == 0) return null;
            return list.Find(x => x.IsActive && (x.Name ?? "").Equals(state, oic) || (x.ShortName ?? "").Equals(state, oic));
        }
    }
}