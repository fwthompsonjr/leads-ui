namespace legallead.jdbc.entities
{
    public class SearchIsPaidDto : BaseDto
    {
        public bool? IsPaid { get; set; }
        public bool? IsDownloaded { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("IsPaid", Comparison)) return IsPaid;
                if (fieldName.Equals("IsDownloaded", Comparison)) return IsDownloaded;
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
                if (fieldName.Equals("IsPaid", Comparison)) { IsPaid = ChangeType<bool?>(value); }
                if (fieldName.Equals("IsDownloaded", Comparison)) { IsDownloaded = ChangeType<bool?>(value); }
            }
        }
    }
}