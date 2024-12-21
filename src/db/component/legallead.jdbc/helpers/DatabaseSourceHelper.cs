namespace legallead.jdbc.helpers
{
    internal static class DatabaseSourceHelper
    {
        internal static string GetConfiguration()
        {
            const string line = "\t{ 'name': '~0', 'source': '~1' }";
            var arr = new string[]
            {
                "[",
                string.Concat(line.Replace("~0", "app").Replace("~1", "defaultdb"), ","),
                string.Concat(line.Replace("~0", "error").Replace("~1", "wlogpermissions"), ","),
                line.Replace("~0", "appdb").Replace("~1", "appdb"),
                "]"
            };
            var content = string.Join(nwl, arr);
            return content.Replace("'", '"'.ToString());

        }
        private static readonly string nwl = Environment.NewLine;
    }
}
