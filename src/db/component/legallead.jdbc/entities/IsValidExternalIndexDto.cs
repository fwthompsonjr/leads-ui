namespace legallead.jdbc.entities
{
    public class IsValidExternalIndexDto : BaseDto
    {
        public bool? IsFound { get; set; }
        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("IsFound", Comparison)) return IsFound;
                return null;
            }
            set
            {
                if (field == null) return;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return;
                if (fieldName.Equals("Id", Comparison)) Id = ChangeType<string>(value) ?? string.Empty;
                if (fieldName.Equals("IsFound", Comparison)) IsFound = ChangeType<bool?>(value);
            }
        }
    }
}
