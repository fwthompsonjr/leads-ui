namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "LEADADMINSTATUS")]
    public class GetAdminStatusDto : BaseDto
    {
        public bool? IsAdmin { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                return IsAdmin;
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
                if (fieldName.Equals("IsAdmin", Comparison))
                {
                    IsAdmin = ChangeType<bool?>(value);
                }
            }
        }
    }
}