namespace legallead.jdbc.entities
{
    public class DbCountyAppendLimitDto : BaseDto
    {

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                return Id;
            }
            set
            {
                if (field == null) return;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return;
                Id = ChangeType<string>(value) ?? string.Empty;
            }
        }

    }
}