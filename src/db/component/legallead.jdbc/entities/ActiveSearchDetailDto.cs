namespace legallead.jdbc.entities
{
    public class ActiveSearchDetailDto : BaseDto
    {
        public DateTime? CreateDate { get; set; }
        public string? CountyName { get; set; }
        public string? StateName { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? SearchProgress { get; set; }


        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("CreateDate", Comparison)) return CreateDate;
                if (fieldName.Equals("CountyName", Comparison)) return CountyName;
                if (fieldName.Equals("StateName", Comparison)) return StateName;
                if (fieldName.Equals("StartDate", Comparison)) return StartDate;
                if (fieldName.Equals("EndDate", Comparison)) return EndDate;
                return SearchProgress;
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
                if (fieldName.Equals("CreateDate", Comparison)) { CreateDate = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("CountyName", Comparison)) { CountyName = ChangeType<string?>(value); return; }
                if (fieldName.Equals("StateName", Comparison)) { StateName = ChangeType<string?>(value); return; }
                if (fieldName.Equals("StartDate", Comparison)) { StartDate = ChangeType<string?>(value); return; }
                if (fieldName.Equals("EndDate", Comparison)) { EndDate = ChangeType<string?>(value); return; }
                if (fieldName.Equals("SearchProgress", Comparison)) { SearchProgress = ChangeType<string?>(value); }
            }
        }

    }
}
