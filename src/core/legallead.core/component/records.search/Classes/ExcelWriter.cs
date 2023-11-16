﻿using legallead.records.search.Dto;
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
        private IExcelFileWriter _fileWriter;

        private IExcelFileWriter FileWriter
        {
            get { return _fileWriter ?? (_fileWriter = new ExcelFileWriter()); }
            set { _fileWriter = value; }
        }

        public ExcelWriter(IExcelFileWriter fileWriter = null)
        {
            FileWriter = fileWriter;
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

            var writer = new ExcelWriter();
            var extXml = CommonKeyIndexes.ExtensionXml;
            var extFile = CommonKeyIndexes.ExtensionXlsx;
            var tmpFileName = fetchResult.Result.Replace(extXml, extFile);
            if (true)
            {
                // Debug.Assert(fetchResult.PeopleList.Count > 0);
                var htmlCaseList = fetchResult.CaseList;
                Debug.Assert(string.IsNullOrEmpty(htmlCaseList) == false);
            }

            using (var workBook = writer.ConvertToPersonTable(
                addressList: fetchResult.PeopleList,
                worksheetName: "Addresses",
                saveFile: false,
                outputFileName: tmpFileName,
                websiteId: fetchResult.WebsiteId))
            {
                writer.ConvertToDataTable(
                excelPackage: workBook,
                htmlTable: fetchResult.CaseList,
                worksheetName: "CaseData",
                saveFile: true,
                outputFileName: tmpFileName,
                websiteId: fetchResult.WebsiteId);
            }
        }

        public ExcelPackage ConvertToPersonTable(
            List<PersonAddress> addressList,
            string worksheetName,
            ExcelPackage excelPackage = null,
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

            var countyName = SettingsManager
                .GetNavigation().Find(x => x.Id == websiteId)
                .Name.Replace("County", "")
                .Replace("Criminal", "")
                .Trim();
            var pck = excelPackage ?? new ExcelPackage();
            var wsDt = pck.Workbook.Worksheets.Add(worksheetName);
            var rowIndex = 1;
            int countyIndex = 0;
            int courtAddressIndex = 0;
            int courtNameId = 10;
            var specialList = new Dictionary<string, string>
            {
                { "firstname", "fname" },
                { "lastname", "lname" }
            };
            List<string> localFieldList = default;
            foreach (var item in addressList)
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
                if (rowIndex == 1)
                {
                    // write header
                    var headerIndex = 1;
                    foreach (var field in localFieldList)
                    {
                        var heading = wsDt.Cells[rowIndex, headerIndex];
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
                var culture = CultureInfo.CurrentCulture;
                for (int i = 0; i < localFieldList.Count; i++)
                {
                    var field = localFieldList[i];
                    var fieldName = specialList.ContainsKey(field) & websiteId == 30 ? specialList[field] : field;
                    var content = item[fieldName];
                    var cleaner = new StringBuilder(content);
                    cleaner.Replace(Environment.NewLine, " ");
                    cleaner.Replace(((char)10).ToString(culture), " ");
                    cleaner.Replace(((char)13).ToString(culture), " ");
                    cleaner.Replace("  ", " ");
                    content = cleaner.ToString().Trim();
                    wsDt.Cells[rowIndex, i + 1].Value = content;
                }

                wsDt.Cells[rowIndex, countyIndex].Value = countyName;
                wsDt.Cells[rowIndex, courtAddressIndex].Value =
                    LookupCountyAddress(websiteId,
                    wsDt.Cells[rowIndex, courtNameId].Value.ToString());
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
            var list = SearchSettingDto.GetCourtLookupList.CourtLocations;
            var court = list.FirstOrDefault(
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
            var courtLocation = court.Courts
                .FirstOrDefault(a =>
                    a.Name.Equals(value, ccic)
                    | a.FullName.Equals(value, ccic));
            if (courtLocation != null)
            {
                return courtLocation.Address;
            }

            var blankLocation = court.Courts
                .FirstOrDefault(a => a.Name.Equals("default", ccic));

            return blankLocation != null ? blankLocation.Address : string.Empty;
        }

        public ExcelPackage ConvertToDataTable(
            string htmlTable,
            string worksheetName,
            ExcelPackage excelPackage = null,
            bool saveFile = false,
            string outputFileName = "",
            int websiteId = 1)
        {
            var pck = excelPackage ?? new ExcelPackage();

            var namedStyle = pck.Workbook.Styles.CreateNamedStyle("HyperLink");
            namedStyle.Style.Font.UnderLine = true;
            namedStyle.Style.Font.Color.SetColor(Color.Blue);
            var wsDt = pck.Workbook.Worksheets.Add(worksheetName);
            var webNav = SettingsManager.GetNavigation()
                 .FirstOrDefault(n => n.Id.Equals(websiteId));
            var hyperPrefix = webNav?.Keys.FirstOrDefault(h => h.Name.Equals("hlinkUri", StringComparison.CurrentCultureIgnoreCase));

            var sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<body id='body-wrapper'>");
            sb.AppendLine(htmlTable);
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            // HTML-table to an excel worksheet
            var doc = XmlDocProvider.GetDoc(sb.ToString());
            var body = doc.DocumentElement.SelectSingleNode("body");
            var table = body.FirstChild;
            var rows = table.ChildNodes.Cast<XmlNode>().ToList();
            var rowIndex = 1;
            rowIndex = GenerateExcelOutput(wsDt, hyperPrefix, rows, rowIndex, table, websiteId);
            // format rows
            ApplyGridFormatting(websiteId, "caselayout", wsDt, rows);

            // save data
            if (saveFile) { FileWriter.SaveAs(pck, outputFileName); }
            return pck;
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

            foreach (var item in table.ChildNodes.Cast<XmlNode>().ToList())
            {
                wsDt.Cells[rowIndex, Case].Value = item.ChildNodes[Case - 1].InnerText;
                wsDt.Cells[rowIndex, Style].Value = item.ChildNodes[Style - 1].InnerText;
                wsDt.Cells[rowIndex, DateFiled].Value = item.ChildNodes[DateFiled - 1].InnerText;
                wsDt.Cells[rowIndex, Court].Value = item.ChildNodes[Court - 1].InnerText;
                wsDt.Cells[rowIndex, CaseType].Value = item.ChildNodes[CaseType - 1].InnerText;
                wsDt.Cells[rowIndex, Status].Value = item.ChildNodes[Status - 1].InnerText;
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
            foreach (var item in rows)
            {
                var colIndex = 1;
                var tdCollection = item.InnerXml.Contains("<th") ?
                    item.SelectNodes("th").Cast<XmlNode>().ToList() :
                    item.SelectNodes("td").Cast<XmlNode>().ToList();
                if (tdCollection != null)
                {
                    foreach (var td in tdCollection)
                    {
                        var sbb = new StringBuilder();
                        var nodeList = td.ChildNodes.Cast<XmlNode>().ToList();
                        var target = wsDt.Cells[rowIndex, colIndex];
                        nodeList
                            .ForEach(n => sbb.Append(n.InnerText.Trim() + " "));
                        target.Value = sbb.ToString().Trim();
                        colIndex++;
                        if (hyperPrefix == null)
                        {
                            continue;
                        }
                        // does this node contain a hyperlink?
                        var hyperlink = nodeList.FirstOrDefault(x => x.Name.Equals("a", StringComparison.CurrentCultureIgnoreCase));
                        if (hyperlink == null)
                        {
                            continue;
                        }

                        var txHref = hyperlink.Attributes.GetNamedItem("href");
                        if (txHref == null)
                        {
                            continue;
                        }

                        var hlink = string.Format(
                            CultureInfo.CurrentCulture,
                            @"{0}{1}", hyperPrefix.Value, txHref.InnerText);
                        target.Hyperlink = new System.Uri(hlink);
                    }
                }
                rowIndex++;
            }

            return rowIndex;
        }

        private void ApplyGridFormatting<T>(int websiteId,
            string sectionName,
            ExcelWorksheet wsDt,
            System.Collections.Generic.List<T> rows)
        {
            const int rowIndex = 1;
            var isCaseLayout = "people" == sectionName;
            var columns = SettingsManager.GetColumnLayouts(websiteId, sectionName);
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
            // apply borders
            var rcount = websiteId == 30 & sectionName == "caselayout" ? rows.Count + 1 : rows.Count;
            var ccount = isCaseLayout ? columns.Count + 2 : columns.Count;
            var rngCells = wsDt.Cells[1, 1, rcount, ccount];
            var rngTopRow = wsDt.Cells[1, 1, 1, ccount];
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