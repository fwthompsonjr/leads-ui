using legallead.logging.entities;

namespace legallead.logging.tests.testobj
{
    internal class TempDto : CommonBaseDto
    {
        public string? Name { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("Name", Comparison)) return Name;
                return null;
            }
            set
            {
                if (field == null) return;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return;
                if (fieldName.Equals("Id", Comparison))
                {
                    Id = ChangeType<long>(value);
                    return;
                }
                if (fieldName.Equals("Name", Comparison))
                {
                    Name = ChangeType<string>(value);
                }
            }
        }
    }
}