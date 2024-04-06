using legallead.installer.Classes;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace legallead.installer.Interfaces
{
    [ExcludeFromCodeCoverage(Justification = "Process creates file resources, integration testing only")]
    public abstract class ModelStorageBase<T> : IModelStorage<T> where T : class
    {
        public DateTime CreationDate { get; set; }
        public List<T> Detail { get; set; } = [];
        public abstract string Name { get; }
        public bool IsValid
        {
            get
            {
                if (Detail.Count == 0) return false;
                var diff = DateTime.UtcNow.Subtract(CreationDate).TotalMinutes;
                return diff < 0;
            }
        }
        public void Save()
        {
            var isKeyDefined = EnvironmentStorageKey.StorageKeys.TryGetValue(Name, out var storageKey);
            if (!isKeyDefined || string.IsNullOrEmpty(storageKey))
                throw new InvalidOperationException("Unable to save object. Invalid key provided.");
            try
            {
                CreationDate = DateTime.UtcNow.AddHours(4);
                var js = JsonConvert.SerializeObject(this);
                Environment.SetEnvironmentVariable(storageKey, js, EnvironmentVariableTarget.User);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public List<T>? Find()
        {
            var isKeyDefined = EnvironmentStorageKey.StorageKeys.TryGetValue(Name, out var storageKey);
            if (!isKeyDefined || string.IsNullOrEmpty(storageKey))
                return null;
            try
            {
                var js = Environment.GetEnvironmentVariable(storageKey, EnvironmentVariableTarget.User);
                if (string.IsNullOrEmpty(js)) return null;
                var model = JsonConvert.DeserializeObject(js, GetType());
                if (model is not IModelStorage<T> storage) return null;
                if (!storage.IsValid) return null;
                return storage.Detail;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
