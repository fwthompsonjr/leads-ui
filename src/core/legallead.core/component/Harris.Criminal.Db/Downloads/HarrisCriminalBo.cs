namespace Harris.Criminal.Db.Downloads
{
    public class HarrisCriminalBo : HarrisCriminalDto
    {
        private const string dteFmt = "yyyyMMdd";
        private static readonly DateTime MinDate = new(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private string? _literalCaseStatus;
        public DateTime DateFiled => FilingDate.ToExactDate(dteFmt, MinDate);

        public string LiteralCaseStatus
        {
            get
            {
                if (_literalCaseStatus != null)
                {
                    return _literalCaseStatus;
                }
                const StringComparison oic = StringComparison.OrdinalIgnoreCase;
                const string fieldName = "cst";
                var tableName = $"HCC.Tables.{fieldName}";
                int fieldId = AliasNames.IndexOf(fieldName);
                var actualFieldValue = this[fieldId]?.Trim();
                if (string.IsNullOrEmpty(actualFieldValue))
                {
                    _literalCaseStatus = string.Empty;
                    return _literalCaseStatus;
                }
                var dataTable = Startup.References.DataList.Find(f => f.Name.Equals(tableName, oic));
                var dataItem = dataTable?.Data.Find(d => d.Code.Equals(actualFieldValue, oic));
                _literalCaseStatus = dataItem?.Literal ?? string.Empty;
                return _literalCaseStatus;
            }
        }

        public static List<HarrisCriminalBo> Map(List<HarrisCriminalDto> data)
        {
            if (data == null)
            {
                return new();
            }

            if (!data.Any())
            {
                return new();
            }

            var result = new List<HarrisCriminalBo>();
            var fields = FieldNames;
            foreach (var item in data)
            {
                var bo = new HarrisCriminalBo();
                fields.ForEach(f => { bo[f] = item[f]; });
                result.Add(bo);
            }
            return result;
        }
    }
}