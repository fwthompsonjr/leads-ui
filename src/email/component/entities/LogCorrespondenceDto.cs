using legallead.email.attributes;

namespace legallead.email.entities
{
    [TargetTable("PRC_EMAIL_LOG_SEND")]
    public class LogCorrespondenceDto : BaseDto
    {
        public string? JsonData { get; set; }

        public override object? this[string field]
        {
            get
            {
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                return JsonData;
            }
            set
            {
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return;
                if (fieldName.Equals("Id", Comparison))
                {
                    Id = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("JsonData", Comparison)) { JsonData = ChangeType<string?>(value); }
            }
        }
    }
}