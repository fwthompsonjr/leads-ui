

using legallead.records.search.Models;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace legallead.permissions.api.Services
{
    public static class NonPersonQueueService
    {
        public static string GetPeople(QueueNonPersonBo q)
        {
            var bytes = q.ExcelData;
            if (bytes == null || bytes.Length == 0) return string.Empty;
            try
            {
                using var ms = new MemoryStream(bytes);
                var package = new ExcelPackage(ms);
                var wk = package.Workbook.Worksheets.FirstOrDefault();
                if (wk == null) return string.Empty;
                var rows = wk.Dimension.Rows;
                var cols = wk.Dimension.Columns;
                var fieldmap = GetDictionary(wk, cols);
                var people = new List<PersonAddress>();
                for (int r = 2; r <= rows; r++)
                {
                    var person = new PersonAddress();
                    for (int c = 1; c <= cols; c++)
                    {
                        var current = Convert.ToString(wk.Cells[r, c].Value);
                        PopulateField(person, fieldmap, c, current);
                    }
                    person = person.ToCalculatedNames();
                    people.Add(person);
                }
                return JsonConvert.SerializeObject(people);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static Dictionary<string, int> GetDictionary(ExcelWorksheet ws, int count)
        {
            var fields = ("Name,FirstName,LastName,Zip,Address1,Address2,Address3,CaseNumber,CaseStyle" +
                ",DateFiled,Court,CaseType,Plantiff,County,CourtAddress").Split(',').ToList();
            var data = new Dictionary<string, int>();
            for (var c = 1; c <= count; c++)
            {
                var cell = Convert.ToString(ws.Cells[1, c].Value) ?? string.Empty;
                var indx = cell == null ? -2 : fields.IndexOf(cell);
                if (indx >= 0 && !string.IsNullOrEmpty(cell))
                {
                    data.Add(cell, c);
                }
            }
            return data;
        }

        private static void PopulateField(PersonAddress person, Dictionary<string, int> fields, int fieldNumber, string? current)
        {
            var values = fields.Values.ToList();
            if (string.IsNullOrEmpty(current)) return;
            if (!values.Contains(fieldNumber)) return;
            var field = fields.First(x => x.Value.Equals(fieldNumber));
            switch (field.Key)
            {
                case "Name":
                    person.Name = current;
                    break;
                case "Zip":
                    person.Zip = current;
                    break;
                case "Address1":
                    person.Address1 = current;
                    break;
                case "Address2":
                    person.Address2 = current;
                    break;
                case "Address3":
                    person.Address3 = current;
                    break;
                case "CaseNumber":
                    person.CaseNumber = current;
                    break;
                case "CaseStyle":
                    person.CaseStyle = current;
                    break;
                case "DateFiled":
                    person.DateFiled = current;
                    break;
                case "Court":
                    person.Court = current;
                    break;
                case "CaseType":
                    person.CaseType = current;
                    break;
                case "Plantiff":
                    person.Plantiff = current;
                    break;
                default:
                    break;
            }

        }
    }
}

