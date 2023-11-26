using legallead.records.search.Dto;
using legallead.records.search.Interfaces;
using legallead.records.search.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Xml;

namespace legallead.records.search.Classes
{
    public class ExcelWriter
    {
        private IExcelFileWriter? _fileWriter;

        private IExcelFileWriter FileWriter
        {
            get { return _fileWriter ??= new ExcelFileWriter(); }
            set { _fileWriter = value; }
        }

        public ExcelWriter(IExcelFileWriter? fileWriter = null)
        {
            FileWriter = fileWriter ?? new ExcelFileWriter();
        }

        public static void WriteToExcel(WebFetchResult fetchResult)
        {
            if (fetchResult == null)
            {
                throw new ArgumentNullException(nameof(fetchResult));
            }
            if (fetchResult.WebsiteId < 1)
            {
                fetchResult.WebsiteId = 1;
            }

            ExcelWriter writer = new();
            string extXml = CommonKeyIndexes.ExtensionXml;
            string extFile = CommonKeyIndexes.ExtensionXlsx;
            string tmpFileName = fetchResult.Result.Replace(extXml, extFile);
            if (true)
            {
                string htmlCaseList = fetchResult.CaseList;
                Debug.Assert(!string.IsNullOrEmpty(htmlCaseList));
            }

            using ExcelPackage workBook = writer.ConvertToPersonTable(
                addressList: fetchResult.PeopleList,
                worksheetName: "Addresses",
                saveFile: false,
                outputFileName: tmpFileName,
                websiteId: fetchResult.WebsiteId);
            writer.ConvertToDataTable(
            excelPackage: workBook,
            htmlTable: fetchResult.CaseList,
            worksheetName: "CaseData",
            saveFile: true,
            outputFileName: tmpFileName,
            websiteId: fetchResult.WebsiteId);
        }

        public ExcelPackage ConvertToPersonTable(
            List<PersonAddress> addressList,
            string worksheetName,
            ExcelPackage? excelPackage = null,
            bool saveFile = false,
            string outputFileName = "",
            int websiteId = 1)
        {
            if (addressList == null)
            {
                throw new ArgumentNullException(nameof(addressList));
            }

            if (string.IsNullOrEmpty(worksheetName))
            {
                throw new ArgumentNullException(nameof(worksheetName));
            }

            string countyName = SettingsManager
                .GetNavigation().Find(x => x.Id == websiteId)
                !.Name.Replace("County", "")
                .Replace("Criminal", "")
                .Trim();
            ExcelPackage pck = excelPackage ?? new ExcelPackage();
            ExcelWorksheet wsDt = pck.Workbook.Worksheets.Add(worksheetName);
            int rowIndex = 1;
            int countyIndex = 0;
            int courtAddressIndex = 0;
            int courtNameId = 10;
            Dictionary<string, string> specialList = new()
            {
                { "firstname", "fname" },
                { "lastname", "lname" }
            };
            List<string>? localFieldList = default;
            foreach (PersonAddress item in addressList)
            {
                if (addressList.IndexOf(item) == 0)
                {
                    localFieldList = item.FieldList;
                    if (websiteId == 40)
                    {
                        localFieldList.RemoveAt(localFieldList.Count - 1);
                        localFieldList.RemoveAt(localFieldList.Count - 1);
                    }
                }
                if (localFieldList == null) break;
                if (rowIndex == 1)
                {
                    // write header
                    int headerIndex = 1;
                    foreach (string field in localFieldList)
                    {
                        ExcelRange heading = wsDt.Cells[rowIndex, headerIndex];
                        heading.Value = field;
                        headerIndex++;
                    }
                    // append new column for County
                    countyIndex = headerIndex;
                    courtAddressIndex = headerIndex + 1;
                    wsDt.Cells[rowIndex, countyIndex].Value = "County";
                    wsDt.Cells[rowIndex, courtAddressIndex].Value = "CourtAddress";
                    rowIndex++;
                }
                CultureInfo culture = CultureInfo.CurrentCulture;
                for (int i = 0; i < localFieldList.Count; i++)
                {
                    string field = localFieldList[i];
                    string fieldName = specialList.ContainsKey(field) & websiteId == 30 ? specialList[field] : field;
                    string content = item[fieldName];
                    StringBuilder cleaner = new(content);
                    cleaner.Replace(Environment.NewLine, " ");
                    cleaner.Replace(((char)10).ToString(culture), " ");
                    cleaner.Replace(((char)13).ToString(culture), " ");
                    cleaner.Replace("  ", " ");
                    content = cleaner.ToString().Trim();
                    wsDt.Cells[rowIndex, i + 1].Value = content;
                }
                var cellvalue = Convert.ToString(wsDt.Cells[rowIndex, courtNameId].Value) ?? "";
                wsDt.Cells[rowIndex, countyIndex].Value = countyName;
                wsDt.Cells[rowIndex, courtAddressIndex].Value = LookupCountyAddress(websiteId, cellvalue);
                rowIndex++;
            }
            addressList.Add(new PersonAddress());
            ApplyGridFormatting(websiteId, "people", wsDt, addressList);
            if (saveFile) { FileWriter.SaveAs(pck, outputFileName); }
            return pck;
        }

        private static string LookupCountyAddress(int websiteId, string value)
        {
            const StringComparison ccic = StringComparison.CurrentCultureIgnoreCase;
            IList<CourtLocation> list = SearchSettingDto.GetCourtLookupList.CourtLocations;
            CourtLocation? court = list.FirstOrDefault(
                c => c.Id.Equals(websiteId.ToString("0", new System.Globalization.NumberFormatInfo()), ccic));
            if (court == null)
            {
                return string.Empty;
            }

            court.Courts
                .Where(a => string.IsNullOrEmpty(a.FullName))
                .ToList()
                .ForEach(b => b.FullName = string.Empty);
            court.Courts
                .Where(a => string.IsNullOrEmpty(a.Name))
                .ToList()
                .ForEach(b => b.Name = string.Empty);
            Court? courtLocation = court.Courts
                .FirstOrDefault(a =>
                    a.Name.Equals(value, ccic)
                    | a.FullName.Equals(value, ccic));
            if (courtLocation != null)
            {
                return courtLocation.Address;
            }

            Court? blankLocation = court.Courts
                .FirstOrDefault(a => a.Name.Equals("default", ccic));

            return blankLocation != null ? blankLocation.Address : string.Empty;
        }

        public ExcelPackage ConvertToDataTable(
            string htmlTable,
            string worksheetName,
            ExcelPackage? excelPackage = null,
            bool saveFile = false,
            string outputFileName = "",
            int websiteId = 1)
        {
            ExcelPackage pck = excelPackage ?? new ExcelPackage();

            OfficeOpenXml.Style.XmlAccess.ExcelNamedStyleXml namedStyle = pck.Workbook.Styles.CreateNamedStyle("HyperLink");
            namedStyle.Style.Font.UnderLine = true;
            namedStyle.Style.Font.Color.SetColor(Color.Blue);
            ExcelWorksheet wsDt = pck.Workbook.Worksheets.Add(worksheetName);
            WebNavigationParameter? webNav = SettingsManager.GetNavigation()
                 .Find(n => n.Id.Equals(websiteId));
            WebNavigationKey? hyperPrefix = webNav?.Keys.Find(h => h.Name.Equals("hlinkUri", StringComparison.CurrentCultureIgnoreCase));

            StringBuilder sb = new();
            sb.AppendLine("<html>");
            sb.AppendLine("<body id='body-wrapper'>");
            sb.AppendLine(htmlTable);
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            // HTML-table to an excel worksheet
            XmlDocument doc = XmlDocProvider.GetDoc(sb.ToString());
            XmlNode? body = doc.DocumentElement!.SelectSingleNode("body");
            XmlNode? table = body!.FirstChild;
            List<XmlNode> rows = table!.ChildNodes.Cast<XmlNode>().ToList();
            int rowIndex = 1;
            rowIndex = GenerateExcelOutput(wsDt, hyperPrefix ?? new(), rows, rowIndex, table, websiteId);
            // format rows
            ApplyGridFormatting(websiteId, "caselayout", wsDt, rows);

            // save data
            if (saveFile) { FileWriter.SaveAs(pck, outputFileName); }
            return pck;
        }

        private static string GetNodeInnerText(XmlNode? node, int index)
        {
            if (node == null || !node.HasChildNodes || node.ChildNodes.Count < index) return string.Empty;
            var child = node.ChildNodes[index];
            return child?.InnerText ?? string.Empty;
        }

        private static int GenerateExcelOutput(ExcelWorksheet wsDt,
            XmlNode table
            )
        {
            int rowIndex = 1;
            const int Case = 1;
            const int Style = 2;
            const int DateFiled = 3;
            const int Court = 4;
            const int CaseType = 5;
            const int Status = 6;

            wsDt.Cells[rowIndex, Case].Value = "Case";
            wsDt.Cells[rowIndex, Style].Value = "Style";
            wsDt.Cells[rowIndex, DateFiled].Value = "DateFiled";
            wsDt.Cells[rowIndex, Court].Value = "Court";
            wsDt.Cells[rowIndex, CaseType].Value = "CaseType";
            wsDt.Cells[rowIndex, Status].Value = "Status";

            rowIndex++;

            foreach (XmlNode? item in table.ChildNodes.Cast<XmlNode>().ToList())
            {
                wsDt.Cells[rowIndex, Case].Value = GetNodeInnerText(item, Case - 1);
                wsDt.Cells[rowIndex, Style].Value = GetNodeInnerText(item, Style - 1);
                wsDt.Cells[rowIndex, DateFiled].Value = GetNodeInnerText(item, DateFiled - 1);
                wsDt.Cells[rowIndex, Court].Value = GetNodeInnerText(item, Court - 1);
                wsDt.Cells[rowIndex, CaseType].Value = GetNodeInnerText(item, CaseType - 1);
                wsDt.Cells[rowIndex, Status].Value = GetNodeInnerText(item, Status - 1);
                rowIndex++;
            }

            return rowIndex;
        }

        private static int GenerateExcelOutput(ExcelWorksheet wsDt,
            WebNavigationKey hyperPrefix,
            List<XmlNode> rows,
            int rowIndex,
            XmlNode table,
            int websiteId)
        {
            if (websiteId == 30)
            {
                return GenerateExcelOutput(wsDt, table);
            }
            foreach (XmlNode item in rows)
            {
                int colIndex = 1;
                List<XmlNode>? tdCollection = item.InnerXml.Contains("<th") ?
                    item.SelectNodes("th")?.Cast<XmlNode>().ToList() :
                    item.SelectNodes("td")?.Cast<XmlNode>().ToList();
                if (tdCollection != null)
                {
                    foreach (XmlNode? td in tdCollection)
                    {
                        StringBuilder sbb = new();
                        List<XmlNode> nodeList = td.ChildNodes.Cast<XmlNode>().ToList();
                        ExcelRange target = wsDt.Cells[rowIndex, colIndex];
                        nodeList
                            .ForEach(n => sbb.Append(n.InnerText.Trim() + " "));
                        target.Value = sbb.ToString().Trim();
                        colIndex++;
                        if (hyperPrefix == null)
                        {
                            continue;
                        }
                        // does this node contain a hyperlink?
                        XmlNode? hyperlink = nodeList.Find(x => x.Name.Equals("a", StringComparison.CurrentCultureIgnoreCase));
                        if (hyperlink == null)
                        {
                            continue;
                        }

                        XmlNode? txHref = hyperlink.Attributes?.GetNamedItem("href");
                        if (txHref == null)
                        {
                            continue;
                        }

                        string hlink = string.Format(
                            CultureInfo.CurrentCulture,
                            @"{0}{1}", hyperPrefix.Value, txHref.InnerText);
                        if (Uri.TryCreate(hlink, UriKind.RelativeOrAbsolute, out var url))
                        {
                            target.Hyperlink = url;
                        }
                    }
                }
                rowIndex++;
            }

            return rowIndex;
        }

        private static void ApplyGridFormatting<T>(int websiteId,
            string sectionName,
            ExcelWorksheet wsDt,
            System.Collections.Generic.List<T> rows)
        {
            const int rowIndex = 1;
            bool isCaseLayout = "people" == sectionName;
            List<ExcelColumnLayout>? columns = SettingsManager.GetColumnLayouts(websiteId, sectionName);
            if (columns != null)
            {
                // format first row
                for (int cidx = 0; cidx < columns.Count; cidx++)
                {
                    wsDt.Cells[rowIndex, cidx + 1].Value = columns[cidx].Name;
                    wsDt.Column(cidx + 1).Width = columns[cidx].ColumnWidth;
                }
                if (isCaseLayout)
                {
                    wsDt.Column(columns.Count + 2).Width = columns[10].ColumnWidth;
                    wsDt.Column(columns.Count + 1).Width = columns[10].ColumnWidth;
                }
            }
            if (columns == null) return;
            // apply borders
            int rcount = websiteId == 30 & sectionName == "caselayout" ? rows.Count + 1 : rows.Count;
            int ccount = isCaseLayout ? columns.Count + 2 : columns.Count;
            ExcelRange rngCells = wsDt.Cells[1, 1, rcount, ccount];
            ExcelRange rngTopRow = wsDt.Cells[1, 1, 1, ccount];
            const OfficeOpenXml.Style.ExcelBorderStyle xlThin = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

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

            rngCells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            rngCells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
            rngCells.Style.WrapText = true;
        }
    }
}