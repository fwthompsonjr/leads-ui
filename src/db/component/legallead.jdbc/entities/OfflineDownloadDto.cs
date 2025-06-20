﻿namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "OFF_DOWNLOAD_RSP")]
    public class OfflineDownloadDto : BaseDto
    {
        public string? RequestId { get; set; }
        public bool? CanDownload { get; set; }
        public int? CountyId { get; set; }
        public string? CountyName { get; set; }
        public string? CourtType { get; set; }
        public int? ItemCount { get; set; }
        public string? Workload { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("RequestId", Comparison)) return RequestId;
                if (fieldName.Equals("CanDownload", Comparison)) return CanDownload;
                if (fieldName.Equals("CountyId", Comparison)) return CountyId;
                if (fieldName.Equals("CountyName", Comparison)) return CountyName;
                if (fieldName.Equals("CourtType", Comparison)) return CourtType;
                if (fieldName.Equals("ItemCount", Comparison)) return ItemCount;
                return Workload;
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
                if (fieldName.Equals("CanDownload", Comparison)) { CanDownload = ChangeType<bool?>(value); return; }
                if (fieldName.Equals("RequestId", Comparison)) { RequestId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("CountyId", Comparison)) { CountyId = ChangeType<int?>(value); return; }
                if (fieldName.Equals("CountyName", Comparison)) { CountyName = ChangeType<string?>(value); return; }
                if (fieldName.Equals("CourtType", Comparison)) { CourtType = ChangeType<string?>(value); return; }
                if (fieldName.Equals("ItemCount", Comparison)) { ItemCount = ChangeType<int?>(value); return; }
                if (fieldName.Equals("Workload", Comparison)) { Workload = ChangeType<string?>(value); }
            }
        }
    }
}