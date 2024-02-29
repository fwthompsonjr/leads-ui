namespace legallead.jdbc.entities
{
    public class DownloadHistoryDto : BaseDto
    {
        public string? UserId { get; set; }
        public string? SearchId { get; set; }
        public decimal? Price { get; set; }
        public int? RowCount { get; set; }
        public string? InvoiceId { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public bool? AllowRollback { get; set; }
        public int? RollbackCount { get; set; }
        public DateTime? CreateDate { get; set; }


        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("UserId", Comparison)) return UserId;
                if (fieldName.Equals("SearchId", Comparison)) return SearchId;
                if (fieldName.Equals("Price", Comparison)) return Price;
                if (fieldName.Equals("RowCount", Comparison)) return RowCount;
                if (fieldName.Equals("InvoiceId", Comparison)) return InvoiceId;
                if (fieldName.Equals("PurchaseDate", Comparison)) return PurchaseDate;
                if (fieldName.Equals("AllowRollback", Comparison)) return AllowRollback;
                if (fieldName.Equals("RollbackCount", Comparison)) return RollbackCount;
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
                if (fieldName.Equals("UserId", Comparison)) { UserId = ChangeType<string>(value); return; }
                if (fieldName.Equals("SearchId", Comparison)) { SearchId = ChangeType<string>(value); return; }
                if (fieldName.Equals("Price", Comparison)) { Price = ChangeType<decimal?>(value); return; }
                if (fieldName.Equals("RowCount", Comparison)) { RowCount = ChangeType<int?>(value); return; }
                if (fieldName.Equals("InvoiceId", Comparison)) { InvoiceId = ChangeType<string>(value); return; }
                if (fieldName.Equals("PurchaseDate", Comparison)) { PurchaseDate = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("AllowRollback", Comparison)) { AllowRollback = ChangeType<bool?>(value); return; }
                if (fieldName.Equals("RollbackCount", Comparison)) { RollbackCount = ChangeType<int?>(value); return; }
                if (fieldName.Equals("CreateDate", Comparison)) { CreateDate = ChangeType<DateTime?>(value); }
            }
        }
    }
}
