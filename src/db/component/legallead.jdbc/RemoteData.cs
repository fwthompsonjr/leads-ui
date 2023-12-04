using System.Diagnostics.CodeAnalysis;

namespace legallead.jdbc
{
    internal static class RemoteData
    {
        private const string PostGresCommand = "Host=lower-sponge-13511.5xj.cockroachlabs.cloud:26257;Username=<username>;Password=<password>;Database=defaultdb";

        private static string[] GetPassCode()
        {
            const string saltLocal = "legal.lead.pgre.passcode";
            const string conversion = "fcxFbnRZ0YtjToqyBHnS4yu3qDdkwHFhk4+L8SllIo1UUDh4hqzEqdqJwEZTeRNP";
            const string vector = "CMFdsdHMyvKbUvcynbzlQg==";
            return CryptoManager.Decrypt(conversion, saltLocal, vector).Split('|');
        }

        public static string GetPostGreString(string environmentVaribleName = "LEGALLEAD_USE_LOCAL")
        {
            if (UseLocalDb(environmentVaribleName)) { return LocalData.GetPostGreString(); }
            var secret = GetPassCode();
            var connection = PostGresCommand;
            connection = connection.Replace("<username>", secret[0]);
            connection = connection.Replace("<password>", secret[1]);
            return connection;
        }

        [ExcludeFromCodeCoverage(Justification = "The public methods that use this private method are fully covered.")]
        private static bool UseLocalDb(string variableName)
        {
            try
            {
                var response = Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine);
                if (response != null && response.Equals("True", StringComparison.OrdinalIgnoreCase)) { return true; }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}