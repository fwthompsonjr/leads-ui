namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "OFF_STATUS_RSP")]
    public class OfflineStatusDto : BaseDto
    {
        public bool? IsCompleted { get; set; }
        public string? RequestId { get; set; }
        public string? Workload { get; set; }
        public string? Cookie { get; set; }
        public string? Message { get; set; }
        public string? OfflineId { get; set; }
        public int? RowCount { get; set; }
        public int? RetryCount { get; set; }
        public string? LeadUserId { get; set; } = string.Empty;
        public string? CountyName { get; set; } = string.Empty;
        public DateTime? SearchStartDate { get; set; }
        public DateTime? SearchEndDate { get; set; }
        public decimal? PercentComplete { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastUpdate { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("IsCompleted", Comparison)) return IsCompleted;
                if (fieldName.Equals("OfflineId", Comparison)) return OfflineId;
                if (fieldName.Equals("RequestId", Comparison)) return RequestId;
                if (fieldName.Equals("LeadUserId", Comparison)) return LeadUserId;
                if (fieldName.Equals("CountyName", Comparison)) return CountyName;
                if (fieldName.Equals("SearchStartDate", Comparison)) return SearchStartDate;
                if (fieldName.Equals("SearchEndDate", Comparison)) return SearchEndDate;
                if (fieldName.Equals("RowCount", Comparison)) return RowCount;
                if (fieldName.Equals("PercentComplete", Comparison)) return PercentComplete;
                if (fieldName.Equals("RetryCount", Comparison)) return RetryCount;
                if (fieldName.Equals("Message", Comparison)) return Message;
                if (fieldName.Equals("Workload", Comparison)) return Workload;
                if (fieldName.Equals("Cookie", Comparison)) return Cookie;
                if (fieldName.Equals("CreateDate", Comparison)) return CreateDate;
                return LastUpdate;
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
                if (fieldName.Equals("OfflineId", Comparison)) { OfflineId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("IsCompleted", Comparison)) { IsCompleted = ChangeType<bool?>(value); return; }
                if (fieldName.Equals("RequestId", Comparison)) { RequestId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("LeadUserId", Comparison)) { LeadUserId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("CountyName", Comparison)) { CountyName = ChangeType<string?>(value); return; }
                if (fieldName.Equals("SearchStartDate", Comparison)) { SearchStartDate = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("SearchEndDate", Comparison)) { SearchEndDate = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("RowCount", Comparison)) { RowCount = ChangeType<int?>(value); return; }
                if (fieldName.Equals("PercentComplete", Comparison)) { PercentComplete = ChangeType<decimal?>(value); return; }
                if (fieldName.Equals("RetryCount", Comparison)) { RetryCount = ChangeType<int?>(value); return; }
                if (fieldName.Equals("Message", Comparison)) { Message = ChangeType<string?>(value); return; }
                if (fieldName.Equals("Workload", Comparison)) { Workload = ChangeType<string?>(value); return; }
                if (fieldName.Equals("Cookie", Comparison)) { Cookie = ChangeType<string?>(value); return; }
                if (fieldName.Equals("LastUpdate", Comparison)) { LastUpdate = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("CreateDate", Comparison)) { CreateDate = ChangeType<DateTime?>(value); }
            }
        }
    }
}