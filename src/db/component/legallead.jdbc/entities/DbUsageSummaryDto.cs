namespace legallead.jdbc.entities
{
    public class DbUsageSummaryDto : BaseDto
    {
        public string? UserName { get; set; }
        public string? LeadUserId { get; set; }
        public int? SearchYear { get; set; }
        public int? SearchMonth { get; set; }
        public DateTime? LastSearchDate { get; set; }
        public int? CountyId { get; set; }
        public string? CountyName { get; set; }
        public int? MTD { get; set; }
        public int? MonthlyLimit { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("UserName", Comparison)) return UserName;
                if (fieldName.Equals("LeadUserId", Comparison)) return LeadUserId;
                if (fieldName.Equals("SearchYear", Comparison)) return SearchYear;
                if (fieldName.Equals("SearchMonth", Comparison)) return SearchMonth;
                if (fieldName.Equals("LastSearchDate", Comparison)) return LastSearchDate;
                if (fieldName.Equals("CountyId", Comparison)) return CountyId;
                if (fieldName.Equals("CountyName", Comparison)) return CountyName;
                if (fieldName.Equals("MTD", Comparison)) return MTD;
                return MonthlyLimit;
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
                if (fieldName.Equals("UserName", Comparison)) { UserName = ChangeType<string?>(value); return; }
                if (fieldName.Equals("LeadUserId", Comparison)) { LeadUserId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("SearchYear", Comparison)) { SearchYear = ChangeType<int?>(value); return; }
                if (fieldName.Equals("SearchMonth", Comparison)) { SearchMonth = ChangeType<int?>(value); return; }
                if (fieldName.Equals("LastSearchDate", Comparison)) { LastSearchDate = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("CountyId", Comparison)) { CountyId = ChangeType<int?>(value); return; }
                if (fieldName.Equals("CountyName", Comparison)) { CountyName = ChangeType<string?>(value); return; }
                if (fieldName.Equals("MTD", Comparison)) { MTD = ChangeType<int?>(value); return; }
                if (fieldName.Equals("MonthlyLimit", Comparison)) { MonthlyLimit = ChangeType<int?>(value); }
            }
        }

    }
}