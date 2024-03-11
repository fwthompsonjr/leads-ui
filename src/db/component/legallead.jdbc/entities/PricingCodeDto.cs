namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "PERMISSIONGROUPCODES")]
    public class PricingCodeDto : BaseDto
    {
        public string? PermissionGroupId { get; set; }
        public string? KeyName { get; set; }
        public string? ProductCode { get; set; }
        public string? PriceCodeAnnual { get; set; }
        public string? PriceCodeMonthly { get; set; }
        public string? KeyJs { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateDate { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("PermissionGroupId", Comparison)) return PermissionGroupId;
                if (fieldName.Equals("KeyName", Comparison)) return KeyName;
                if (fieldName.Equals("ProductCode", Comparison)) return ProductCode;
                if (fieldName.Equals("PriceCodeAnnual", Comparison)) return PriceCodeAnnual;
                if (fieldName.Equals("PriceCodeMonthly", Comparison)) return PriceCodeMonthly;
                if (fieldName.Equals("KeyJs", Comparison)) return KeyJs;
                if (fieldName.Equals("IsActive", Comparison)) return IsActive;
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
                if (fieldName.Equals("PermissionGroupId", Comparison)) { PermissionGroupId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("KeyName", Comparison)) { KeyName = ChangeType<string?>(value); return; }
                if (fieldName.Equals("ProductCode", Comparison)) { ProductCode = ChangeType<string?>(value); return; }
                if (fieldName.Equals("PriceCodeAnnual", Comparison)) { PriceCodeAnnual = ChangeType<string?>(value); return; }
                if (fieldName.Equals("PriceCodeMonthly", Comparison)) { PriceCodeMonthly = ChangeType<string?>(value); return; }
                if (fieldName.Equals("KeyJs", Comparison)) { KeyJs = ChangeType<string?>(value); }
                if (fieldName.Equals("IsActive", Comparison)) { IsActive = ChangeType<bool?>(value); return; }
                if (fieldName.Equals("CreateDate", Comparison)) { CreateDate = ChangeType<DateTime?>(value); }
            }
        }
    }
}