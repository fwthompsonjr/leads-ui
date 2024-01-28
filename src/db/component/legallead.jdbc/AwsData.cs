using legallead.jdbc.helpers;
using legallead.jdbc.models;
using Newtonsoft.Json;

namespace legallead.jdbc
{
    internal static class AwsData
    {
        private static readonly object locker = new();
        public static string GetPostGreString(string connectionType = "Local", string databaseName = "app")
        {
            lock (locker)
            {
                DbConnectProvider.Target = connectionType;
                return DbConnectProvider.ConnectionString(databaseName); 
            }
        }
        private static DbConnect DbConnectProvider => _provider ??= GetConnect();
        private static DbConnect? _provider;
        private static readonly string DbConnectString = ConnectionStringHelper.GetConfiguration();
        private static DbConnect GetConnect()
        {
            return JsonConvert.DeserializeObject<DbConnect>(DbConnectString) ?? new();
        }
    }
}