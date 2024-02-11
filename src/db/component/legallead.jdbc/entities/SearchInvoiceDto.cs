namespace legallead.jdbc.entities
{
    public class SearchInvoiceDto : BaseDto
    {
        public string? LineId { get; set; }
        public string? UserId { get; set; }
        public string? ItemType { get; set; }
        public int? ItemCount { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Price { get; set; }
        public string? ReferenceId { get; set; }
        public string? ExternalId { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("LineId", Comparison)) return LineId;
                if (fieldName.Equals("UserId", Comparison)) return UserId;
                if (fieldName.Equals("ItemType", Comparison)) return ItemType;
                if (fieldName.Equals("ItemCount", Comparison)) return ItemCount;
                if (fieldName.Equals("UnitPrice", Comparison)) return UnitPrice;
                if (fieldName.Equals("Price", Comparison)) return Price;
                if (fieldName.Equals("ReferenceId", Comparison)) return ReferenceId;
                if (fieldName.Equals("ExternalId", Comparison)) return ExternalId;
                if (fieldName.Equals("PurchaseDate", Comparison)) return PurchaseDate;
                if (fieldName.Equals("IsDeleted", Comparison)) return IsDeleted;
                if (fieldName.Equals("CreateDate", Comparison)) return CreateDate;
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
                if (fieldName.Equals("LineId", Comparison)) LineId = ChangeType<string>(value);
                if (fieldName.Equals("UserId", Comparison)) UserId = ChangeType<string>(value);
                if (fieldName.Equals("ItemType", Comparison)) ItemType = ChangeType<string>(value);
                if (fieldName.Equals("ItemCount", Comparison)) ItemCount = ChangeType<int?>(value);
                if (fieldName.Equals("UnitPrice", Comparison)) UnitPrice = ChangeType<decimal?>(value);
                if (fieldName.Equals("Price", Comparison)) Price = ChangeType<decimal?>(value);
                if (fieldName.Equals("ReferenceId", Comparison)) ReferenceId = ChangeType<string>(value);
                if (fieldName.Equals("ExternalId", Comparison)) ExternalId = ChangeType<string>(value);
                if (fieldName.Equals("PurchaseDate", Comparison)) PurchaseDate = ChangeType<DateTime?>(value);
                if (fieldName.Equals("IsDeleted", Comparison)) IsDeleted = ChangeType<bool?>(value);
                if (fieldName.Equals("CreateDate", Comparison)) CreateDate = ChangeType<DateTime?>(value);
            }
        }
    }
}