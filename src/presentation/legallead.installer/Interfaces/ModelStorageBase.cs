using legallead.installer.Bo;
using legallead.installer.Classes;
using legallead.installer.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

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
            var targets = new[] { EnvironmentVariableTarget.User, EnvironmentVariableTarget.Process }.ToList();
            var isKeyDefined = EnvironmentStorageKey.StorageKeys.TryGetValue(Name, out var storageKey);
            if (!isKeyDefined || string.IsNullOrEmpty(storageKey))
                throw new InvalidOperationException("Unable to save object. Invalid key provided.");
            try
            {
                CreationDate = DateTime.UtcNow.AddHours(4);
                var js = JsonConvert.SerializeObject(this);
                SetValue(storageKey, js, null);
                targets.ForEach(t => SetValue(storageKey, js, t));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public List<T>? Find()
        {
            var targets = new[] { EnvironmentVariableTarget.User, EnvironmentVariableTarget.Process }.ToList();
            var isKeyDefined = EnvironmentStorageKey.StorageKeys.TryGetValue(Name, out var storageKey);
            if (!isKeyDefined || string.IsNullOrEmpty(storageKey))
                return null;
            try
            {
                var js = GetValue(storageKey, null);
                while (string.IsNullOrEmpty(js))
                {
                    foreach (var t in targets)
                    {
                        js = GetValue(storageKey, t);
                        if (!string.IsNullOrEmpty(js)) break;
                    }
                }
                if (string.IsNullOrEmpty(js)) return null;
                var model = DeserializeObject(js, GetType());
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

        private static object? DeserializeObject(string json, Type type)
        {
            if (!ModelStorageMapper.Translators.TryGetValue(type, out var conversion))
            {
                var model = JsonConvert.DeserializeObject(json, type);
                if (model is not IModelStorage<T> _) return null;
                return model;
            }
            var translation = JsonConvert.DeserializeObject(json, conversion);
            var obj = JsonConvert.DeserializeObject(json, type);
            if (obj is not IModelStorage<T> storage) return null;
            if (translation is not ICreateDateProperty creationDt) return null;
            var dte = creationDt.CreationDate.Replace("T", " ");
            storage.CreationDate = DateTime.Parse(dte, CultureInfo.InvariantCulture);
            if (storage is ReleaseModelStorage releases && translation is ReleaseModelStorageBo releasesBo)
            {
                releases.Detail = releasesBo.Models();
                return releases;
            }
            return storage;
        }

        private static void SetValue(string keyName, string keyValue, EnvironmentVariableTarget? target)
        {
            try
            {
                var location = target ?? EnvironmentVariableTarget.Process;
                if (target == null)
                    Environment.SetEnvironmentVariable(keyName, keyValue);
                else
                    Environment.SetEnvironmentVariable(keyName, keyValue, location);
            }
            catch
            {
                // no action is needed
            }
        }

        private static string? GetValue(string keyName, EnvironmentVariableTarget? target)
        {
            try
            {
                var location = target ?? EnvironmentVariableTarget.Process;
                if (target == null)
                    return Environment.GetEnvironmentVariable(keyName);
                else
                    return Environment.GetEnvironmentVariable(keyName, location);
            }
            catch
            {
                return null;
            }
        }

    }
}
