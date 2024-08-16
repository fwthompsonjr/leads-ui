namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "APPSETTINGS")]
    public class AppSettingDto : BaseDto
    {
        public string? KeyName { get; set; }
        public string? KeyValue { get; set; }
        public decimal? Version { get; set; }
        public DateTime? CreateDate { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("KeyName", Comparison)) return KeyName;
                if (fieldName.Equals("KeyValue", Comparison)) return KeyValue;
                if (fieldName.Equals("Version", Comparison)) return Version;
                return CreateDate;
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
                if (fieldName.Equals("KeyName", Comparison))
                {
                    KeyName = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("KeyValue", Comparison))
                {
                    KeyValue = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("Version", Comparison)) { Version = ChangeType<decimal?>(value); }
                if (fieldName.Equals("CreateDate", Comparison)) CreateDate = ChangeType<DateTime?>(value);
            }
        }
    }
}