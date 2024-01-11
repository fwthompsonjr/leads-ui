namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "SEARCH")]
    public class SearchDto : BaseDto
    {
        public string? Name { get; set; }
        public string? UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ExpectedRows { get; set; }
        public DateTime? CreateDate { get; set; }


        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("UserId", Comparison)) return UserId;
                if (fieldName.Equals("Name", Comparison)) return Name;
                if (fieldName.Equals("StartDate", Comparison)) return StartDate;
                if (fieldName.Equals("EndDate", Comparison)) return EndDate;
                if (fieldName.Equals("ExpectedRows", Comparison)) return ExpectedRows;
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
                if (fieldName.Equals("UserId", Comparison))
                {
                    UserId = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("Name", Comparison))
                {
                    Name = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("StartDate", Comparison))
                {
                    StartDate = ChangeType<DateTime?>(value);
                    return;
                }
                if (fieldName.Equals("EndDate", Comparison))
                {
                    EndDate = ChangeType<DateTime?>(value);
                }
                if (fieldName.Equals("ExpectedRows", Comparison))
                {
                    ExpectedRows = ChangeType<int?>(value);
                }
                if (fieldName.Equals("CreateDate", Comparison))
                {
                    CreateDate = ChangeType<DateTime?>(value);
                }
            }
        }
    }
}
