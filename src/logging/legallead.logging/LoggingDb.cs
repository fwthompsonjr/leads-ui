namespace legallead.logging
{
    internal static class LoggingDb
    {
        private const string Endpoint = "database-leads.cmmu8tkizri9.us-east-2.rds.amazonaws.com";
        private const string PostGresCommand = "server=<localhost>;user=<username>;password=<password>;Database=wlogpermissions;port=3306;";

        private static string[] GetPassCode()
        {
            const string saltLocal = "legal.lead.awsd.passcode";
            const string conversion = "+mnWfJqr8rejN8Gi1GcnibZj+/zTaRqWvP5oQ8uCwwU=";
            const string vector = "X+jEaHnsFfQ3IZgF23b12w==";
            return CryptoContent.Decrypt(conversion, saltLocal, vector).Split('|');
        }

        public static string GetConnectionString()
        {
            if (!string.IsNullOrWhiteSpace(_dbconnection))
                return _dbconnection;

            var secret = GetPassCode();
            var connection = PostGresCommand;
            connection = connection.Replace("<localhost>", $"{Endpoint}");
            connection = connection.Replace("<username>", secret[0]);
            connection = connection.Replace("<password>", secret[1]);
            _dbconnection = connection;
            return _dbconnection;
        }

        private static string? _dbconnection;
    }
}