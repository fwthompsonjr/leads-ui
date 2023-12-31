﻿namespace legallead.records.search.Dto
{
    public class HarrisCaseSearchDto
    {
        private string? _uniqueIndex;
        public string CaseNumber { get; set; } = string.Empty;
        public string DateFiled { get; set; } = string.Empty;
        public string Court { get; set; } = string.Empty;

        public string DateFormat { get; set; } = string.Empty;

        public string UniqueIndex() => GetUniqueIndex();

        private string GetUniqueIndex()
        {
            string dateFiled = GetDate(DateFiled, DateFormat);
            string caseNumber = GetText(CaseNumber, "0000");
            string court = GetText(Court, "0000");

            _uniqueIndex = $"{dateFiled}~{caseNumber}~{court}";
            return _uniqueIndex;
        }

        private static string GetText(string text, string textDefault)
        {
            if (string.IsNullOrEmpty(text))
            {
                return textDefault;
            }
            return text;
        }

        private static string GetDate(string dateFiled, string dateFormat)
        {
            string currentDate = DateTime.Now.ToString("s");
            if (string.IsNullOrEmpty(dateFiled) | string.IsNullOrEmpty(dateFormat))
            {
                dateFiled = currentDate;
            }
            DateTime date = dateFiled.ToExactDate(dateFormat, DateTime.MaxValue);
            if (date != DateTime.MaxValue)
            {
                return date.ToString("s");
            }
            return currentDate;
        }
    }
}