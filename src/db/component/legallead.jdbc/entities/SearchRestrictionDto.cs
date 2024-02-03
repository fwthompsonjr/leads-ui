namespace legallead.jdbc.entities
{
    public class SearchRestrictionDto : BaseDto
    {

        public bool? IsLocked { get; set; }
        public string? Reason { get; set; }
        public int? MaxPerMonth { get; set; }
        public int? MaxPerYear { get; set; }
        public int? ThisMonth { get; set; }
        public int? ThisYear { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("IsLocked", Comparison)) return IsLocked;
                if (fieldName.Equals("Reason", Comparison)) return Reason;
                if (fieldName.Equals("MaxPerMonth", Comparison)) return MaxPerMonth;
                if (fieldName.Equals("MaxPerYear", Comparison)) return MaxPerYear;
                if (fieldName.Equals("ThisMonth", Comparison)) return ThisMonth;
                if (fieldName.Equals("ThisYear", Comparison)) return ThisYear;
                return null;
            }
            set
            {
                if (field == null) return;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return;
                if (fieldName.Equals("Id", Comparison)) Id = ChangeType<string>(value) ?? string.Empty;
                if (fieldName.Equals("IsLocked", Comparison)) IsLocked = ChangeType<bool?>(value);
                if (fieldName.Equals("Reason", Comparison)) Reason = ChangeType<string>(value) ?? string.Empty;
                if (fieldName.Equals("MaxPerMonth", Comparison)) MaxPerMonth = ChangeType<int?>(value);
                if (fieldName.Equals("MaxPerYear", Comparison)) MaxPerYear = ChangeType<int?>(value);
                if (fieldName.Equals("ThisMonth", Comparison)) ThisMonth = ChangeType<int?>(value);
                if (fieldName.Equals("ThisYear", Comparison)) ThisYear = ChangeType<int?>(value);
            }
        }
    }
}
