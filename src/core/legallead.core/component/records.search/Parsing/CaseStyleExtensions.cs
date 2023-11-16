using legallead.harriscriminal.db.Tables;
using legallead.records.search.Models;

namespace legallead.records.search.Parsing
{
    public static class CaseStyleExtensions
    {
        private static readonly CaseStyleDbParser DbParser = new();

        public static IEnumerable<HLinkDataRow> ToHLinkData(this List<CaseStyleDb> cases)
        {
            if (cases == null || cases.Count == 0)
            {
                return Array.Empty<HLinkDataRow>();
            }

            List<HLinkDataRow> data = new();
            CaseStyleDbParser parser = DbParser;
            foreach (CaseStyleDb item in cases)
            {
                parser.Data = item.Style;

                HLinkDataRow dta = new()
                {
                    WebsiteId = 40,
                    DateFiled = item.FileDate,
                    Court = item.Court,
                    CaseType = item.TypeOfActionOrOffense,
                    CaseStyle = item.Style,
                    Case = item.CaseNumber
                };
                if (parser.CanParse())
                {
                    Dto.ParseCaseStyleDbDto parsed = parser.Parse();
                    dta.CaseStyle = parsed.CaseData;
                    dta.Defendant = parsed.Defendant;
                }
                data.Add(dta);
            }
            return data;
        }

        public static IEnumerable<PersonAddress> ToPersonAddress(this HLinkDataRow dataRow, List<HarrisCriminalDto> dtos)
        {
            if (dataRow == null)
            {
                return Array.Empty<PersonAddress>();
            }

            List<HarrisCriminalDto> matched = dtos;
            PersonAddress drow = FromCase(dataRow);
            if (matched == null || !matched.Any())
            {
                return new PersonAddress[] { drow };
            }

            List<PersonAddress> records = new();
            CaseStyleDbParser parser = DbParser;
            parser.Data = dataRow.CaseStyle;
            Dto.ParseCaseStyleDbDto parsed = parser.Parse();
            foreach (HarrisCriminalDto item in matched)
            {
                PersonAddress dto = new()
                {
                    Name = item.DefendantName,
                    Address1 = $"{item.DefendantStreetNumber} {item.DefendantStreetName}",
                    Address2 = $"{item.DefendantCity}, {item.DefendantState} {item.DefendantZip}",
                    Address3 = string.Empty,
                    CaseNumber = drow.CaseNumber,
                    CaseStyle = drow.CaseStyle,
                    CaseType = drow.CaseType,
                    Court = drow.Court,
                    DateFiled = drow.DateFiled,
                    Plantiff = parsed.Plantiff,
                    Zip = item.DefendantZip
                };
                if (dto.Address2.Equals(", ", StringComparison.CurrentCultureIgnoreCase))
                {
                    dto.Address2 = string.Empty;
                }
                if (string.IsNullOrEmpty(dto.Zip))
                {
                    dto.Zip = "00000";
                }
                records.Add(dto);
            }
            return records;
        }

        private static PersonAddress FromCase(HLinkDataRow dataRow)
        {
            const string notFoundName = "Found, Not";
            if (dataRow == null)
            {
                return new PersonAddress();
            }

            CaseStyleDbParser parser = DbParser;
            parser.Data = dataRow.CaseStyle;
            Dto.ParseCaseStyleDbDto parsed = parser.Parse();
            bool canParse = parser.CanParse();
            string name = canParse ? parsed.Defendant : notFoundName;
            if (string.IsNullOrEmpty(name))
            {
                name = notFoundName;
            }
            return new PersonAddress
            {
                Name = name,
                Address1 = "- Not Found -",
                Address2 = "NA, 00000",
                CaseNumber = dataRow.Case,
                CaseStyle = dataRow.CaseStyle,
                CaseType = dataRow.CaseType,
                Court = dataRow.Court,
                DateFiled = dataRow.DateFiled,
                Plantiff = parsed.Plantiff,
                Zip = "00000"
            };
        }
    }
}