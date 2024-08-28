namespace legallead.records.search.DriverFactory
{
    internal static class TheEnvironment
    {
        public static string? GetHomeFolder()
        {
            var home = Environment.GetEnvironmentVariable("HOME");
            var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!string.IsNullOrEmpty(home)) return home;
            if (!string.IsNullOrEmpty(local)) return local;
            return null;
        }
    }
}
