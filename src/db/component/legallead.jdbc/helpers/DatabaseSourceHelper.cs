namespace legallead.jdbc.helpers
{
    internal static class DatabaseSourceHelper
    {
        internal static string GetConfiguration()
        {
            var content = "["
                          + nwl
                          + "\t{ 'name': 'app', 'source': 'defaultdb' },"
                          + nwl
                          + "\t{ 'name': 'error', 'source': 'wlogpermissions' }"
                          + nwl
                          + "]";
            return content.Replace("'", '"'.ToString());

        }
        private static string nwl = Environment.NewLine;
    }
}
