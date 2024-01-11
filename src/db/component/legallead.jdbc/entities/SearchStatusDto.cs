namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "SEARCHSTATUS")]
    public class SearchStatusDto : BaseDto
    {
        public string? SearchId { get; set; }
        public int? LineNbr { get; set; }
        public string? Line { get; set; }
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
                if (fieldName.Equals("LineNbr", Comparison)) return LineNbr;
                if (fieldName.Equals("Line", Comparison)) return Line;
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
                if (fieldName.Equals("LineNbr", Comparison))
                {
                    LineNbr = ChangeType<int?>(value);
                    return;
                }
                if (fieldName.Equals("Line", Comparison))
                {
                    Line = ChangeType<string>(value);
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