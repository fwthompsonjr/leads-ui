using legallead.installer.Interfaces;

namespace legallead.installer.Classes
{
    public class LocalsParser : ILocalsParser
    {
        public string GetLatest(string installed, string appName)
        {
            if (string.IsNullOrEmpty(installed)) return string.Empty;
            if (!installed.Contains(appName, StringComparison.OrdinalIgnoreCase)) return string.Empty;
            var lines = installed.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            bool dataFound = false;
            string find = $"- {appName}";
            const string doubleDash = "--";
            const string singleDash = "- ";
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            var locals = new List<string>();
            foreach (var line in lines)
            {
                if (line.Contains(find, oic))
                {
                    dataFound = true;
                    continue;
                }
                if (dataFound && line.Contains(doubleDash, oic))
                {
                    locals.Add(line.Replace(doubleDash, "").Trim());
                }
                if (dataFound && locals.Count > 0 && line.Contains(singleDash, oic))
                {
                    break;
                }
            }
            var haslines = locals.Exists(x => x.Contains(' '));
            if (!haslines) { return string.Empty; }
            var item = locals[^1];
            return item.Split(' ')[0];
        }
    }
}
