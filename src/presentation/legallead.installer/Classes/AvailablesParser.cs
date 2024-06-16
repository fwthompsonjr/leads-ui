using legallead.installer.Interfaces;

namespace legallead.installer.Classes
{
    public class AvailablesParser : IAvailablesParser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Sonar Qube",
            "S4158:Collection is known to be empty here",
            Justification = "False positive")]
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
                if (line.Contains(appName, oic) && line.Contains(doubleDash, oic))
                {
                    locals.Add(line.Replace(doubleDash, "").Trim());
                }
            });
            var haslines = locals.Exists(x => x.Contains(' '));
            if (!haslines) { return string.Empty; }
            var item = locals[^1];
            var items = item.Split(' ');
            var version = items[^1];
            return version;
        }
    }
}