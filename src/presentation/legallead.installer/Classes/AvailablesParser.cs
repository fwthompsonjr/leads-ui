using legallead.installer.Interfaces;

namespace legallead.installer.Classes
{
    public class AvailablesParser : IAvailablesParser
    {
        public string GetLatest(string available, string appName)
        {
            if (string.IsNullOrEmpty(available)) return string.Empty;
            if (!available.Contains(appName, StringComparison.OrdinalIgnoreCase)) return string.Empty;
            var lines = available.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
            const string doubleDash = "--";
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            var locals = new List<string>();
            lines.ForEach(line =>
            {
                if (line.Contains(appName, oic))
                {
                    locals.Add(line.Replace(doubleDash, "").Trim());
                }
            });
            var item = locals.LastOrDefault();
            if (item == null) { return string.Empty; }
            var items = item.Split(' ');
            var version = items.LastOrDefault() ?? string.Empty;
            return version;
        }
    }
}