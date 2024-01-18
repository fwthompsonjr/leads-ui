using legallead.records.search.Tools;

namespace legallead.records.search.Classes
{
    public class DentonAddressSummary
    {
        private string? _personType;
        private string? _personName;
        private string? _address;

        public string? PersonHTML { get; set; }
        public string? AddressHTML { get; set; }

        public string? PersonType()
        {
            if (_personType != null) return _personType;
            if (string.IsNullOrEmpty(PersonHTML))
            {
                _personType = string.Empty;
                return string.Empty;
            }

            var node = (PersonHTML ?? "").GetNode();
            if (node == null)
            {
                _personType = string.Empty;
                return string.Empty;
            }

            var ths = node.SelectNodes("//th");
            if (ths == null || ths.Count < 1)
            {
                _personType = string.Empty;
                return string.Empty;
            }
            _personType = ths[0].InnerText.Trim();
            return _personType;
        }
        public string? PersonName()
        {
            if (_personName != null) return _personName;
            if (string.IsNullOrEmpty(PersonHTML))
            {
                _personName = string.Empty;
                return string.Empty;
            }

            var node = (PersonHTML ?? "").GetNode();
            if (node == null)
            {
                _personName = string.Empty;
                return string.Empty;
            }

            var ths = node.SelectNodes("//th");
            if (ths == null || ths.Count < 1)
            {
                _personName = string.Empty;
                return string.Empty;
            }
            _personName = ths[1].InnerText.Trim();
            return _personName;
        }

        public string? Address()
        {
            if (_address != null) return _address;
            if (string.IsNullOrEmpty(AddressHTML))
            {
                _address = string.Empty;
                return string.Empty;
            }

            var node = (AddressHTML ?? "").GetNode();
            if (node == null)
            {
                _address = string.Empty;
                return string.Empty;
            }

            var ths = node.SelectNodes("//td");
            if (ths == null || ths.Count < 1)
            {
                _address = string.Empty;
                return string.Empty;
            }
            _address = FormatAddress(ths[0].InnerHtml);
            return _address;
        }
        public void Calculate()
        {
            if (AddressHTML == null || PersonHTML == null) return;
            _ = PersonType();
            _ = PersonName();
            _ = Address();
        }
        private static string FormatAddress(string html)
        {
            const char pipe = '|';
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            var line = html.Replace("&nbsp;", " ").Trim();
            line = line.Replace("<br>", pipe.ToString());
            var parts = line.Split(pipe, StringSplitOptions.RemoveEmptyEntries).ToList();
            parts.ForEach(part => { _ = part.Trim(); });
            line = string.Join(pipe.ToString(), parts);
            return line;
        }
    }
}
