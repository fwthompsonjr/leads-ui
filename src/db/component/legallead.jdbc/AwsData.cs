using legallead.jdbc.models;
using Newtonsoft.Json;

namespace legallead.jdbc
{
    internal static class AwsData
    {
        public static string GetPostGreString(string connectionType = "Local", string databaseName = "app")
        {
            DbConnectProvider.Target = connectionType;
            return DbConnectProvider.ConnectionString(databaseName);
        }
        private static DbConnect DbConnectProvider => _provider ??= GetConnect();
        private static DbConnect? _provider;
        private static readonly string DbConnectString = Properties.Resources.connectionstring_json;
        private static DbConnect GetConnect()
        {
            return JsonConvert.DeserializeObject<DbConnect>(DbConnectString) ?? new();
        }
    }
}