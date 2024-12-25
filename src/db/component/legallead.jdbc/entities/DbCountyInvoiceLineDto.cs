namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "DBCOUNTYINVOICELINE")]
    public class DbCountyInvoiceLineDto : BaseDto
    {
        public string? InvoiceId { get; set; }
        public int? LineNbr { get; set; }
        public int? ItemCount { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? Total { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("InvoiceId", Comparison)) return InvoiceId;
                if (fieldName.Equals("LineNbr", Comparison)) return LineNbr;
                if (fieldName.Equals("Description", Comparison)) return Description;
                if (fieldName.Equals("ItemCount", Comparison)) return ItemCount;
                if (fieldName.Equals("Price", Comparison)) return Price;
                return Total;
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
                if (fieldName.Equals("InvoiceId", Comparison)) { InvoiceId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("LineNbr", Comparison)) { LineNbr = ChangeType<int?>(value); return; }
                if (fieldName.Equals("Description", Comparison)) { Description = ChangeType<string?>(value); return; }
                if (fieldName.Equals("ItemCount", Comparison)) { ItemCount = ChangeType<int?>(value); return; }
                if (fieldName.Equals("Price", Comparison)) { Price = ChangeType<decimal?>(value); return; }
                if (fieldName.Equals("Total", Comparison)) { Total = ChangeType<decimal?>(value); }
            }
        }

    }
}