using legallead.jdbc.models;
using Newtonsoft.Json;

namespace legallead.logging
{
    internal static class LoggingDb
    {
        public static string GetConnectionString(string connectionType = "Test", string databaseName = "error")
        {
            ErConnectProvider.Target = connectionType;
            return ErConnectProvider.ConnectionString(databaseName);
        }

        private static ErConnect ErConnectProvider => _provider ??= GetConnect();
        private static ErConnect? _provider;
        private static readonly string ErConnectString = Properties.Resources.connectionstring_json;
        private static ErConnect GetConnect()
        {
            return JsonConvert.DeserializeObject<ErConnect>(ErConnectString) ?? new();
        }
    }
}