namespace legallead.jdbc.entities
{
    public class LevelPaymentDto : BaseDto
    {
        public string? UserId { get; set; }
        public string? ExternalId { get; set; }
        public string? LevelName { get; set; }
        public string? PriceType { get; set; }
        public decimal? Price { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? ServiceFee { get; set; }
        public decimal? SubscriptionAmount { get; set; }
        public DateTime? CompletionDate { get; set; }
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
                if (fieldName.Equals("ExternalId", Comparison)) return ExternalId;
                if (fieldName.Equals("LevelName", Comparison)) return LevelName;
                if (fieldName.Equals("PriceType", Comparison)) return PriceType;
                if (fieldName.Equals("Price", Comparison)) return Price;
                if (fieldName.Equals("TaxAmount", Comparison)) return TaxAmount;
                if (fieldName.Equals("ServiceFee", Comparison)) return ServiceFee;
                if (fieldName.Equals("SubscriptionAmount", Comparison)) return SubscriptionAmount;
                if (fieldName.Equals("CompletionDate", Comparison)) return CompletionDate;
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
                if (fieldName.Equals("UserId", Comparison)) { UserId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("ExternalId", Comparison)) { ExternalId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("LevelName", Comparison)) { LevelName = ChangeType<string?>(value); return; }
                if (fieldName.Equals("PriceType", Comparison)) { PriceType = ChangeType<string?>(value); return; }
                if (fieldName.Equals("Price", Comparison)) { Price = ChangeType<decimal?>(value); return; }
                if (fieldName.Equals("TaxAmount", Comparison)) { TaxAmount = ChangeType<decimal?>(value); return; }
                if (fieldName.Equals("ServiceFee", Comparison)) { ServiceFee = ChangeType<decimal?>(value); return; }
                if (fieldName.Equals("SubscriptionAmount", Comparison)) { SubscriptionAmount = ChangeType<decimal?>(value); return; }
                if (fieldName.Equals("CompletionDate", Comparison)) { CompletionDate = ChangeType<DateTime?>(value); }
                if (fieldName.Equals("CreateDate", Comparison)) { CreateDate = ChangeType<DateTime?>(value); }
            }
        }
    }
}