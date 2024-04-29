using legallead.email.attributes;

namespace legallead.email.entities
{
    [TargetTable("PRC_EMAIL_LOG_SUCCESS")]
    public class LogCorrespondenceSuccessDto : BaseDto
    {
        public override object? this[string field]
        {
            get
            {
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                return Id;
            }
            set
            {
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return;
                Id = ChangeType<string>(value) ?? string.Empty;
            }
        }
    }
}
