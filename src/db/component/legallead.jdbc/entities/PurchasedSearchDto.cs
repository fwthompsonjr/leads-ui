namespace legallead.jdbc.entities
{
    public class PurchasedSearchDto : BaseDto
    {
        public DateTime? PurchaseDate { get; set; }
        public string? ReferenceId { get; set; }
        public string? ExternalId { get; set; }
        public string? ItemType { get; set; }
        public int? ItemCount { get; set; }
        public decimal? Price { get; set; }
        public string? StatusText { get; set; }


        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("PurchaseDate", Comparison)) return PurchaseDate;
                if (fieldName.Equals("ReferenceId", Comparison)) return ReferenceId;
                if (fieldName.Equals("ExternalId", Comparison)) return ExternalId;
                if (fieldName.Equals("ItemType", Comparison)) return ItemType;
                if (fieldName.Equals("ItemCount", Comparison)) return ItemCount;
                if (fieldName.Equals("Price", Comparison)) return Price;
                if (fieldName.Equals("StatusText", Comparison)) return StatusText;

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
                if (fieldName.Equals("PurchaseDate", Comparison)) { PurchaseDate = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("ReferenceId", Comparison)) { ReferenceId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("ExternalId", Comparison)) { ExternalId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("ItemType", Comparison)) { ItemType = ChangeType<string?>(value); return; }
                if (fieldName.Equals("ItemCount", Comparison)) { ItemCount = ChangeType<int?>(value); return; }
                if (fieldName.Equals("Price", Comparison)) { Price = ChangeType<decimal?>(value); }
                if (fieldName.Equals("StatusText", Comparison)) { StatusText = ChangeType<string?>(value); }
            }
        }

    }
}
