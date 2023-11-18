using System.Text;

namespace legallead.records.search.Models
{
    using WebUtl = System.Net.WebUtility;

    public static class ModelExtensions
    {
        private static readonly string Defendant =
            ResourceTable.GetText(ResourceType.FindDefendant, ResourceKeyIndex.Defendant);

        private const StringComparison ccic = StringComparison.CurrentCultureIgnoreCase;

        public static List<HLinkDataRow> ConvertToDataRow(this CaseRowData source)
        {
            if (source == null)
            {
                return new();
            }

            List<HLinkDataRow> dest = new();
            List<CaseDataAddress> defentdants = source.CaseDataAddresses
                .Where(x => x.Role.Equals(Defendant, ccic))
                .ToList();
            foreach (CaseDataAddress? person in defentdants)
            {
                HLinkDataRow row = new()
                {
                    WebsiteId = source.RowId,
                    Address = GetAddress(person),
                    Case = person.Case,
                    CaseStyle = source.Style,
                    CaseType = source.TypeDesc,
                    Court = source.Court,
                    DateFiled = source.FileDate,
                    Defendant = GetPerson(person),
                    Data = source.Status
                };
                dest.Add(row);
            }
            return dest;
        }

        public static string GetPerson(this CaseDataAddress source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            char pipe = '|';
            string pipeString = "|";
            const string noMatch = "No Person Associated";
            string person = source.Party;
            if (string.IsNullOrEmpty(person)) { person = noMatch; }
            if (person.EndsWith(pipeString, ccic))
            {
                person = person[..^1];
            }
            List<string> pieces = person.Split(pipe)
                .ToList().FindAll(s => !string.IsNullOrEmpty(s));
            if (!pieces.Any())
            {
                return person;
            }
            return pieces[0].Trim();
        }

        public static string GetAddress(this CaseDataAddress source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            char pipe = '|';
            string pipeString = "|";
            const string noMatch = "No Match Found|Not Matched 00000";
            string address = source.Party;
            if (string.IsNullOrEmpty(address))
            {
                return noMatch;
            }
            address = address.Trim();
            if (address.EndsWith(pipeString, ccic))
            {
                address = address[..^1];
            }
            List<string> pieces = address.Split(pipe)
                .ToList().FindAll(s => !string.IsNullOrEmpty(s));
            if (!pieces.Any())
            {
                return noMatch;
            }
            pieces.ForEach(x => x = x.Trim());
            address = string.Empty;
            // get the person part of this address
            for (int i = 0; i < pieces.Count; i++)
            {
                if (i == 0)
                {
                    continue;
                }

                string piece = pieces[i].Trim();
                if (string.IsNullOrEmpty(address))
                {
                    address = piece;
                }
                else
                {
                    address = (address + pipeString + piece);
                }
            }
            return address;
        }

        public static string ToHtml(this List<CaseRowData> source)
        {
            StringBuilder template = new();
            template.Append("<table>");
            source?.ForEach(s =>
                {
                    template.AppendLine(s.ToHtml());
                });
            template.Append("</table>");
            return template.ToString();
        }

        public static string ToHtml(this CaseRowData source)
        {
            StringBuilder template = new();
            template.AppendLine("<tr>");
            template.AppendLine("<td>[Case]</td>");
            template.AppendLine("<td>[Style]</td>");
            template.AppendLine("<td>[DateFiled]</td>");
            template.AppendLine("<td>[Court]</td>");
            template.AppendLine("<td>[CaseType]</td>");
            template.AppendLine("<td>[Status]</td>");
            template.AppendLine("</tr>");
            if (source == null)
            {
                return template.ToString();
            }

            template.Replace("[RowIndex]", WebUtl.HtmlEncode(source.RowId.ToString()));
            template.Replace("[Case]", WebUtl.HtmlEncode(source.Case));
            template.Replace("[Style]", WebUtl.HtmlEncode(source.Style));
            template.Replace("[DateFiled]", WebUtl.HtmlEncode(source.FileDate));
            template.Replace("[Court]", WebUtl.HtmlEncode(source.Court));
            template.Replace("[CaseType]", WebUtl.HtmlEncode(source.TypeDesc));
            template.Replace("[Status]", WebUtl.HtmlEncode(source.Status));
            return template.ToString();
        }

        public static string ToHtml(this HLinkDataRow source)
        {
            StringBuilder template = new();
            template.AppendLine("<tr>");
            template.AppendLine("<td>[Case]</td>");
            template.AppendLine("<td>[Style]</td>");
            template.AppendLine("<td>[DateFiled]</td>");
            template.AppendLine("<td>[Court]</td>");
            template.AppendLine("<td>[CaseType]</td>");
            template.AppendLine("<td>[Status]</td>");
            template.AppendLine("</tr>");
            if (source == null)
            {
                return template.ToString();
            }

            template.Replace("[RowIndex]", WebUtl.HtmlEncode(source.WebsiteId.ToString()));
            template.Replace("[Case]", WebUtl.HtmlEncode(source.Case));
            template.Replace("[Style]", WebUtl.HtmlEncode(source.CaseStyle));
            template.Replace("[DateFiled]", WebUtl.HtmlEncode(source.DateFiled));
            template.Replace("[Court]", WebUtl.HtmlEncode(source.Court));
            template.Replace("[CaseType]", WebUtl.HtmlEncode(source.CaseType));
            template.Replace("[Status]", WebUtl.HtmlEncode(source.Data));
            return template.ToString();
        }

        public static string ToHtml(this List<HLinkDataRow> source)
        {
            StringBuilder template = new();
            template.Append("<table>");
            source?.ForEach(s =>
                {
                    template.AppendLine(s.ToHtml());
                });
            template.Append("</table>");
            return template.ToString();
        }

        public static string ToHtml(this PersonAddress source)
        {
            StringBuilder template = new();
            template.AppendLine("<tr>");
            template.AppendLine("<td>[Case]</td>");
            template.AppendLine("<td>[Style]</td>");
            template.AppendLine("<td>[DateFiled]</td>");
            template.AppendLine("<td>[Court]</td>");
            template.AppendLine("<td>[CaseType]</td>");
            template.AppendLine("<td>[Status]</td>");
            template.AppendLine("</tr>");
            if (source == null)
            {
                return template.ToString();
            }

            template.Replace("[Case]", WebUtl.HtmlEncode(source.CaseNumber));
            template.Replace("[Style]", WebUtl.HtmlEncode(source.CaseStyle));
            template.Replace("[DateFiled]", WebUtl.HtmlEncode(source.DateFiled));
            template.Replace("[Court]", WebUtl.HtmlEncode(source.Court));
            template.Replace("[CaseType]", WebUtl.HtmlEncode(source.CaseType));
            template.Replace("[Status]", WebUtl.HtmlEncode(source.Status));
            return template.ToString();
        }

        public static string ToHtml(this List<PersonAddress> source)
        {
            StringBuilder template = new();
            template.Append("<table>");
            source?.ForEach(s =>
                {
                    template.AppendLine(s.ToHtml());
                });
            template.Append("</table>");
            return template.ToString();
        }

        public static PersonAddress ToCalculatedZip(this PersonAddress source)
        {
            if (source == null)
            {
                return new();
            }

            if (string.IsNullOrEmpty(source.Zip))
            {
                return source;
            }

            string zipCode = source.Zip.Trim();
            zipCode = zipCode.Replace(((char)160).ToString(), " ");
            List<string> pieces = zipCode.Split(' ').ToList();
            if (!pieces.Any())
            {
                source.Zip = zipCode;
                return source;
            }
            source.Zip = pieces[^1];
            return source;
        }

        public static PersonAddress ToCalculatedNames(this PersonAddress source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (string.IsNullOrEmpty(source.Name))
            {
                return source;
            }

            string fullName = source.Name.Trim();
            List<string> pieces = fullName.Split(' ').ToList();
            if (!pieces.Any())
            {
                source.CalcFirstName = source.Name;
                source.CalcLastName = string.Empty;
                return source;
            }
            source.CalcFirstName = pieces[0];
            List<string> lasts = pieces.GetRange(1, pieces.Count - 1).ToList();
            source.CalcLastName = string.Join(" ", lasts);
            return source;
        }
    }
}