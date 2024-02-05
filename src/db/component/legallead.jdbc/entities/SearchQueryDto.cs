namespace legallead.jdbc.entities
{
    public class SearchQueryDto : SearchDto
    {
        public string? SearchProgress { get; set; }
        public string? StateCode { get; set; }
        public string? CountyName { get; set; }


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
                if (fieldName.Equals("SearchProgress", Comparison)) return SearchProgress;
                if (fieldName.Equals("StateCode", Comparison)) return StateCode;
                if (fieldName.Equals("CountyName", Comparison)) return CountyName;
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
                    return;
                }
                if (fieldName.Equals("ExpectedRows", Comparison))
                {
                    ExpectedRows = ChangeType<int?>(value);
                    return;
                }
                if (fieldName.Equals("CreateDate", Comparison))
                {
                    CreateDate = ChangeType<DateTime?>(value);
                    return;
                }
                if (fieldName.Equals("SearchProgress", Comparison))
                {
                    SearchProgress = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("StateCode", Comparison))
                {
                    StateCode = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("CountyName", Comparison))
                {
                    CountyName = ChangeType<string>(value);
                }
            }
        }
    }
}
