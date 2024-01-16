namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "SEARCHSTAGING")]
    public class SearchStagingDto : BaseDto
    {
        public string? SearchId { get; set; }
        public string? StagingType { get; set; }
        public int? LineNbr { get; set; }
        public byte[]? LineData { get; set; }
        public string? LineText { get; set; }
        public bool? IsBinary { get; set; }
        public DateTime? CreateDate { get; set; }


        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("SearchId", Comparison)) return SearchId;
                if (fieldName.Equals("StagingType", Comparison)) return StagingType;
                if (fieldName.Equals("LineNbr", Comparison)) return LineNbr;
                if (fieldName.Equals("LineData", Comparison)) return LineData;
                if (fieldName.Equals("LineText", Comparison)) return LineText;
                if (fieldName.Equals("IsBinary", Comparison)) return IsBinary;
                if (fieldName.Equals("CreateDate", Comparison)) return CreateDate;
                return null;
            }
            set
            {
                if (field == null) return;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return;
                if (fieldName.Equals("Id", Comparison))
                {
                    Id = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("SearchId", Comparison))
                {
                    SearchId = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("StagingType", Comparison))
                {
                    StagingType = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("LineNbr", Comparison))
                {
                    LineNbr = ChangeType<int?>(value);
                    return;
                }
                if (fieldName.Equals("LineData", Comparison))
                {
                    LineData = ChangeType<byte[]?>(value);
                    return;
                }
                if (fieldName.Equals("LineText", Comparison))
                {
                    LineText = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("IsBinary", Comparison))
                {
                    IsBinary = ChangeType<bool?>(value);
                    return;
                }
                if (fieldName.Equals("CreateDate", Comparison))
                {
                    CreateDate = ChangeType<DateTime?>(value);
                }
            }
        }
    }
}