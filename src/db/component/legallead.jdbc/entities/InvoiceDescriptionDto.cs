using System.Diagnostics;

namespace legallead.jdbc.entities
{
    public class InvoiceDescriptionDto : BaseDto
    {
        public string? ItemDescription { get; set; }
        public string? County { get; set; }
        public string? StateAbbr { get; set; }
        public DateTime? StartingDt { get; set; }
        public DateTime? EndingDt { get; set; }
        public DateTime? RequestDt { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("ItemDescription", Comparison)) return ItemDescription;
                if (fieldName.Equals("County", Comparison)) return County;
                if (fieldName.Equals("StateAbbr", Comparison)) return StateAbbr;
                if (fieldName.Equals("StartingDt", Comparison)) return StartingDt;
                if (fieldName.Equals("EndingDt", Comparison)) return EndingDt;
                if (fieldName.Equals("RequestDt", Comparison)) return RequestDt;
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
                if (fieldName.Equals("ItemDescription", Comparison)) ItemDescription = ChangeType<string>(value);
                if (fieldName.Equals("County", Comparison)) County = ChangeType<string>(value);
                if (fieldName.Equals("StateAbbr", Comparison)) StateAbbr = ChangeType<string>(value);
                if (fieldName.Equals("StartingDt", Comparison)) StartingDt = ChangeType<DateTime?>(value);
                if (fieldName.Equals("EndingDt", Comparison)) EndingDt = ChangeType<DateTime?>(value);
                if (fieldName.Equals("RequestDt", Comparison)) RequestDt = ChangeType<DateTime?>(value);
            }
        }
    }
}
