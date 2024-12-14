namespace legallead.jdbc.entities
{
    public class DbCountyUsageLimitDto : BaseDto
    {
        public string? LeadUserId { get; set; }
        public int? CountyId { get; set; }
        public bool? IsActive { get; set; }
        public int? MaxRecords { get; set; }
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
                if (fieldName.Equals("CountyId", Comparison)) return CountyId;
                if (fieldName.Equals("IsActive", Comparison)) return IsActive;
                if (fieldName.Equals("MaxRecords", Comparison)) return MaxRecords;
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
                if (fieldName.Equals("CountyId", Comparison)) { CountyId = ChangeType<int?>(value); return; }
                if (fieldName.Equals("LeadUserId", Comparison)) { LeadUserId = ChangeType<string>(value); return; }
                if (fieldName.Equals("IsActive", Comparison)) { IsActive = ChangeType<bool?>(value); return; }
                if (fieldName.Equals("MaxRecords", Comparison)) { MaxRecords = ChangeType<int?>(value); return; }
                if (fieldName.Equals("CompleteDate", Comparison)) { CompleteDate = ChangeType<DateTime?>(value); }
                if (fieldName.Equals("CreateDate", Comparison)) { CreateDate = ChangeType<DateTime?>(value); }
            }
        }

    }
}