namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "LEADUSERINVOICE")]
    public class LeadUserInvoiceDto : BaseDto
    {
        public string? LeadUserId { get; set; }
        public string? RequestId { get; set; }
        public string? InvoiceNbr { get; set; }
        public string? InvoiceUri { get; set; }
        public int? RecordCount { get; set; }
        public decimal? Total { get; set; }
        public DateTime? CompleteDate { get; set; }
        public DateTime? CreateDate { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("LeadUserId", Comparison)) return LeadUserId;
                if (fieldName.Equals("RequestId", Comparison)) return RequestId;
                if (fieldName.Equals("InvoiceNbr", Comparison)) return InvoiceNbr;
                if (fieldName.Equals("InvoiceUri", Comparison)) return InvoiceUri;
                if (fieldName.Equals("RecordCount", Comparison)) return RecordCount;
                if (fieldName.Equals("Total", Comparison)) return Total;
                if (fieldName.Equals("CompleteDate", Comparison)) return CompleteDate;
                return CreateDate;
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
                if (fieldName.Equals("LeadUserId", Comparison)) { LeadUserId = ChangeType<string>(value); return; }
                if (fieldName.Equals("RequestId", Comparison)) { RequestId = ChangeType<string>(value); return; }
                if (fieldName.Equals("InvoiceNbr", Comparison)) { InvoiceNbr = ChangeType<string>(value); return; }
                if (fieldName.Equals("InvoiceUri", Comparison)) { InvoiceUri = ChangeType<string>(value); return; }
                if (fieldName.Equals("RecordCount", Comparison)) { RecordCount = ChangeType<int?>(value); return; }
                if (fieldName.Equals("Total", Comparison)) { Total = ChangeType<decimal?>(value); return; }
                if (fieldName.Equals("CompleteDate", Comparison)) { CompleteDate = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("CreateDate", Comparison)) CreateDate = ChangeType<DateTime?>(value);
            }
        }
    }
}