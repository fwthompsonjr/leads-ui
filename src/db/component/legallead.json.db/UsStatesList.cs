using legallead.json.db.entity;

namespace legallead.json.db
{
    internal static class UsStatesList
    {
        private static bool IsPopulated;
        private static readonly List<UsState> list = new();

        public static List<UsState> All => list;

        public static void Populate(List<UsState> items)
        {
            if (IsPopulated) return;
            list.AddRange(items);
            IsPopulated = true;
        }

        public static bool Verify(string? state)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            if (string.IsNullOrEmpty(state)) return false;
            if (!IsPopulated || list.Count == 0) return true;
            var item = list.Find(x => x.IsActive && (x.Name ?? "").Equals(state, oic) || (x.ShortName ?? "").Equals(state, oic));
            return item != null;
        }

        public static UsState? Find(string? state)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            if (string.IsNullOrEmpty(state)) return null;
            if (!IsPopulated || list.Count == 0) return null;
            return list.Find(x => x.IsActive && (x.Name ?? "").Equals(state, oic) || (x.ShortName ?? "").Equals(state, oic));
        }
    }
}