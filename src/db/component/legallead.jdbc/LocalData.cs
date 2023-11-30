namespace legallead.jdbc
{
    internal static class LocalData
    {
#if LOCAL
        private const string PostGresCommand = "Host=localhost;Username=<username>;Password=<password>;Database=legallead_local";

        private static string[] GetPassCode()
        {
            const string saltLocal = "local.lead.pgre.passcode";
            const string conversion = "PJ+tGCWgm71p1EuzljbETc7jK+cWSiBChGnloUKeMWY=";
            const string vector = "oPo5iwHj+8W68av+FTRbjw==";
            return CryptoManager.Decrypt(conversion, saltLocal, vector).Split('|');
        }

        public static string GetPostGreString()
        {
            var secret = GetPassCode();
            var connection = PostGresCommand;
            connection = connection.Replace("<username>", secret[0]);
            connection = connection.Replace("<password>", secret[1]);
            return connection;
        }
#else
        private const string PostGresCommand = "Host=lower-sponge-13511.5xj.cockroachlabs.cloud:26257;Username=<username>;Password=<password>;Database=testdb";

        private static string[] GetPassCode()
        {
            const string saltLocal = "legal.lead.pgre.passcode";
            const string conversion = "fcxFbnRZ0YtjToqyBHnS4yu3qDdkwHFhk4+L8SllIo1UUDh4hqzEqdqJwEZTeRNP";
            const string vector = "CMFdsdHMyvKbUvcynbzlQg==";
            return CryptoManager.Decrypt(conversion, saltLocal, vector).Split('|');
        }

        public static string GetPostGreString()
        {
            var secret = GetPassCode();
            var connection = PostGresCommand;
            connection = connection.Replace("<username>", secret[0]);
            connection = connection.Replace("<password>", secret[1]);
            return connection;
        }
#endif
    }
}