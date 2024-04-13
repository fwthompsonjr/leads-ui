using legallead.jdbc.entities;
using legallead.permissions.api.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace legallead.permissions.api.Extensions
{
    internal static class ExcelExtensions
    {
        public static byte[]? WriteExcel(PaymentSessionDto dto, List<SearchFinalBo> records)
        {
            const string worksheetName = "Addresses";
            try
            {
                var json = dto.JsText ?? "";
                var obj = JsonConvert.DeserializeObject<PaymentSessionJs>(json) ?? new();
                var package = new ExcelPackage();
                ExcelWorksheet wsDt = package.Workbook.Worksheets.Add(worksheetName);
                ColumnNames.ForEach(x =>
                {
                    var cidx = ColumnNames.IndexOf(x) + 1;
                    wsDt.Cells[1, cidx].Value = x;
                    wsDt.Column(cidx).Width = Convert.ToDouble(ColumnIndexes[x]);
                });
                records.ForEach(r =>
                {
                    var ridx = records.IndexOf(r) + 2;
                    WriteContent(ridx, r, wsDt);
                });
                var searchId = records[0].SearchId;
                var rowcount = records.Count + 1;
                ApplyGridFormatting(rowcount, wsDt);
                WriteProperties(obj, package.Workbook, records.Count.ToString(), searchId);
                using var ms = new MemoryStream();
                package.SaveAs(ms);
                return ms.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }
        [ExcludeFromCodeCoverage(Justification = "Private member is accessed from public method")]
        private static void WriteContent(int ridx, SearchFinalBo source, ExcelWorksheet wsDt)
        {
            ColumnNames.ForEach(x =>
            {
                var cidx = ColumnNames.IndexOf(x) + 1;
                wsDt.Cells[ridx, cidx].Value = GetColumn(x, source);
            });
        }
        [ExcludeFromCodeCoverage(Justification = "Private member is accessed from public method")]
        private static void WriteProperties(PaymentSessionJs dto, ExcelWorkbook wb, string rwcount, string password)
        {
            var props = new Dictionary<string, string>() {
                { "Created", DateTime.UtcNow.ToString("o") },
                { "Comments", dto.Description ?? "Legal Lead Record Search" }
            };

            var keys = CommonProperties.Keys.ToList();
            keys.ForEach(x =>
            {
                var kvalue = CommonProperties[x];
                if (props.ContainsKey(x)) { kvalue = props[x]; }
                switch (x)
                {
                    case "Author":
                        wb.Properties.Author = kvalue;
                        wb.Properties.LastModifiedBy = kvalue;
                        wb.Properties.Subject = GetWbSubject(dto.Description, rwcount);
                        break;
                    case "Title":
                        wb.Properties.Title = kvalue;
                        break;
                    case "Comments":
                        wb.Properties.Comments = kvalue;
                        break;
                    case "Company":
                        wb.Properties.Company = kvalue;
                        break;
                    case "Created":
                        wb.Properties.Created = DateTime.UtcNow;
                        break;
                }
            });
            var wks = wb.Worksheets[0];
            wks.Protection.SetPassword(password);
            wb.Protection.LockStructure = true;
            wb.Protection.SetPassword(password);
        }
        [ExcludeFromCodeCoverage(Justification = "Private member is accessed from public method")]
        private static string GetWbSubject(string? description, string rowcount)
        {
            const string fallbak = "Data inquiry";
            if (description == null) return fallbak;
            if (!description.Contains('-')) return fallbak;
            if (!description.Contains(':')) return fallbak;
            var collection = description.Split('-');
            var countyName = collection[0].Split(':')[1].Trim();
            return $"{countyName}, Records: {rowcount}";
        }
        [ExcludeFromCodeCoverage(Justification = "Private member is accessed from public method")]
        private static void ApplyGridFormatting(int rcount, ExcelWorksheet wsDt)
        {
            int ccount = ColumnNames.Count;
            ExcelRange rngCells = wsDt.Cells[1, 1, rcount, ccount];
            ExcelRange rngTopRow = wsDt.Cells[1, 1, 1, ccount];
            const ExcelBorderStyle xlThin = ExcelBorderStyle.Thin;

            // set header background color
            rngTopRow.Style.Font.Bold = true;
            rngTopRow.Style.Fill.PatternType = ExcelFillStyle.Solid;
            rngTopRow.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

            // set borders
            rngCells.Style.Border.Top.Style = xlThin;
            rngCells.Style.Border.Left.Style = xlThin;
            rngCells.Style.Border.Right.Style = xlThin;
            rngCells.Style.Border.Bottom.Style = xlThin;

            // set alignment

            rngCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            rngCells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            rngCells.Style.WrapText = true;
        }

        [ExcludeFromCodeCoverage(Justification = "Private member is accessed from public method")]
        private static string GetColumn(string name, SearchFinalBo source)
        {
            var response = name switch
            {
                "Name" => source.Name,
                "Zip" => source.Zip,
                "Address1" => source.Address1,
                "Address2" => source.Address2,
                "Address3" => source.Address3,
                "CaseNumber" => source.CaseNumber,
                "DateFiled" => source.DateFiled,
                "Court" => source.Court,
                "CaseType" => source.CaseType,
                "CaseStyle" => source.CaseStyle,
                "FirstName" => source.FirstName,
                "LastName" => source.LastName,
                "Plantiff" => source.Plantiff,
                "County" => source.County,
                "CourtAddress" => source.CourtAddress,
                _ => string.Empty
            };
            return response;
        }

        private static readonly List<string> ColumnNames = new() {
        "Name",
        "FirstName",
        "LastName",
        "Zip",
        "Address1",
        "Address2",
        "Address3",
        "CaseNumber",
        "CaseStyle",
        "DateFiled",
        "Court",
        "CaseType",
        "Plantiff",
        "County",
        "CourtAddress",
        };

        private static readonly Dictionary<string, int> ColumnIndexes = new() {
        { "Name", 30 },
        { "FirstName", 20 },
        { "LastName", 20 },
        { "Zip", 20 },
        { "Address1", 30 },
        { "Address2", 30 },
        { "Address3", 30 },
        { "CaseNumber", 30 },
        { "CaseStyle", 30 },
        { "DateFiled", 30 },
        { "Court", 30 },
        { "CaseType", 30 },
        { "Plantiff", 30 },
        { "County", 20 },
        { "CourtAddress", 50 },
        };

        private static readonly Dictionary<string, string> CommonProperties = new() {
        { "Author", "LegalLead API" },
        { "Title", "Legal Lead Record Search" },
        { "Created", "" },
        { "Comments", "" },
        { "Company", "LegalLead, LLC" },
        };

    }
}
