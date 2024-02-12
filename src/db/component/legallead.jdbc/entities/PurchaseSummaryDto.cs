namespace legallead.jdbc.entities
{
    public class PurchaseSummaryDto : BaseDto
    {
        public string? ExternalId { get; set; }
        public decimal? Price { get; set; }
        public string? ItemType { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("ExternalId", Comparison)) return ExternalId;
                if (fieldName.Equals("Price", Comparison)) return Price;
                if (fieldName.Equals("ItemType", Comparison)) return ItemType;
                if (fieldName.Equals("PurchaseDate", Comparison)) return PurchaseDate;
                if (fieldName.Equals("UserName", Comparison)) return UserName;
                if (fieldName.Equals("Email", Comparison)) return Email;
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
                if (fieldName.Equals("ExternalId", Comparison)) ExternalId = ChangeType<string>(value);
                if (fieldName.Equals("Price", Comparison)) Price = ChangeType<decimal?>(value);
                if (fieldName.Equals("ItemType", Comparison)) ItemType = ChangeType<string>(value);
                if (fieldName.Equals("PurchaseDate", Comparison)) PurchaseDate = ChangeType<DateTime?>(value);
                if (fieldName.Equals("UserName", Comparison)) UserName = ChangeType<string>(value);
                if (fieldName.Equals("Email", Comparison)) Email = ChangeType<string>(value);
            }
        }
    }
}
