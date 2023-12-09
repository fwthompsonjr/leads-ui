using legallead.json.interfaces;
using Newtonsoft.Json;

namespace legallead.json.entities
{
    public abstract class BaseEntity<T> where T : class, IDataEntity, new()
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell",
            "S2743:Static fields should not be used in generic types",
            Justification = "Will Refactor. Quik way to make all calls to read/write thread safe.")]
        private static readonly object locker = new();

        public string? Id { get; set; }
        public string? Name { get; set; }
        public bool IsDeleted { get; set; }

        public void Add(T item, string location)
        {
            _ = Insert(item, location);
        }

        public void Save(T item, string location)
        {
            _ = Update(item, location);
        }

        public void Remove(T item, string location)
        {
            _ = Delete(item, location);
        }

        internal static T? Get(string location, Func<T, bool> expression)
        {
            return Find(location, expression);
        }

        internal static IEnumerable<T>? GetAll(string location, Func<T, bool> expression)
        {
            return FindAll(location, expression);
        }

        protected T Insert(T entity, string location)
        {
            if (string.IsNullOrEmpty(location))
                throw new ArgumentNullException(nameof(location), "Data file is missing or not initialized.");

            if (!string.IsNullOrEmpty(entity.Id))
            {
                throw new ArgumentOutOfRangeException(nameof(entity), "Id is not expected for data insert");
            }
            entity.Id = Guid.NewGuid().ToString();
            var table = GetContent(location);
            table.Add(entity);
            var content = JsonConvert.SerializeObject(table);
            SaveContent(content, location);
            return entity;
        }

        protected T Update(T entity, string location)
        {
            if (string.IsNullOrEmpty(location))
                throw new ArgumentNullException(nameof(location), "Data file is missing or not initialized.");

            if (string.IsNullOrEmpty(entity.Id))
            {
                throw new ArgumentOutOfRangeException(nameof(entity), "Id is required for data update");
            }
            var table = GetContent(location);
            var id = table.FindIndex(x => (x.Id ?? "").Equals(entity.Id, StringComparison.OrdinalIgnoreCase));
            table[id] = entity;
            var content = JsonConvert.SerializeObject(table);
            SaveContent(content, location);
            return entity;
        }

        protected static T? Find(string location, Func<T, bool> expression)
        {
            if (string.IsNullOrEmpty(location))
                throw new FileNotFoundException("Data file is missing or not initialized.");

            var table = GetContent(location);
            return table.Find(x => expression(x));
        }

        protected static IEnumerable<T>? FindAll(string location, Func<T, bool> expression)
        {
            if (string.IsNullOrEmpty(location))
                throw new FileNotFoundException("Data file is missing or not initialized.");

            var table = GetContent(location);
            return table.FindAll(x => expression(x));
        }

        protected T? Delete(T entity, string location)
        {
            if (string.IsNullOrEmpty(location))
                throw new ArgumentNullException(nameof(entity), "Data file is missing or not initialized.");

            if (string.IsNullOrEmpty(entity.Id))
            {
                throw new ArgumentOutOfRangeException(nameof(entity), "Id is required for data delete");
            }
            var table = GetContent(location);
            var found = table.FindIndex(x => (x.Id ?? "").Equals(entity.Id, StringComparison.OrdinalIgnoreCase));
            if (found < 0) return null;
            table.RemoveAt(found);
            var content = JsonConvert.SerializeObject(table);
            SaveContent(content, location);
            return entity;
        }

        /// <summary>
        /// Used to write to file system. Please add polly retry x 3 pow situation
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private static List<T> GetContent(string location)
        {
            lock (locker)
            {
                var content = File.ReadAllText(location);
                return TryDeserialize<List<T>>(content);
            }
        }

        /// <summary>
        /// Used to write to file system. Please add polly retry x 3 pow situation
        /// </summary>
        /// <param name="content"></param>
        /// <param name="location"></param>
        private static void SaveContent(string content, string location)
        {
            lock (locker)
            {
                File.WriteAllText(location, content);
            }
        }

        public static K TryDeserialize<K>(string content) where K : new()
        {
            try
            {
                return JsonConvert.DeserializeObject<K>(content) ?? new();
            }
            catch { return new(); }
        }
    }
}