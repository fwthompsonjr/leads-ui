using legallead.records.search.Models;

namespace legallead.records.search
{
    internal static class ResourceFileService
    {
        public static List<ResourceFileModel> Models => _models;

        public static void Clear()
        {
            lock (_locker) { Models.Clear(); }
        }

        public static bool Exists(string keyName)
        {
            lock (_locker)
            {
                var comparison = StringComparer.OrdinalIgnoreCase;
                var collection = Models.Select(x => x.Name).ToList();
                return collection.Contains(keyName, comparison);
            }
        }
        public static void AddOrUpdate(string keyName, string keyValue, TimeSpan expiration)
        {
            lock (_locker)
            {
                const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
                var found = Models.Find(x => x.Name.Equals(keyName, comparison));
                var expirationDate = DateTime.UtcNow.Add(expiration);
                if (found != null)
                {
                    found.Content = keyValue;
                    found.ExpirationDate = expirationDate;
                    return;
                }
                found = new ResourceFileModel
                {
                    Content = keyValue,
                    ExpirationDate = expirationDate,
                    Name = keyName
                };
                Models.Add(found);
            }
        }

        public static string? Get(string keyName)
        {
            lock (_locker)
            {
                const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
                var found = Models.Find(x => x.Name.Equals(keyName, comparison));
                return found?.Content ?? null;
            }
        }

        private static readonly List<ResourceFileModel> _models = new();
        private static readonly object _locker = new();
    }
}
