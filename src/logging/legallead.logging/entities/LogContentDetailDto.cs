using legallead.logging.attr;

namespace legallead.logging.entities
{
    [LogTable(TableName = "LOGCONTENTDETAIL")]
    public class LogContentDetailDto : CommonBaseDto
    {
        public long? LogContentId { get; set; }
        public int? LineId { get; set; }
        public string? Line { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("LogContentId", Comparison)) return LogContentId;
                if (fieldName.Equals("LineId", Comparison)) return LineId;
                if (fieldName.Equals("Line", Comparison)) return Line;
                return null;
            }
            set
            {
                if (field == null) return;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return;
                if (fieldName.Equals("Id", Comparison)) { Id = ChangeType<long?>(value); return; }
                if (fieldName.Equals("LogContentId", Comparison)) { LogContentId = ChangeType<long?>(value); return; }
                if (fieldName.Equals("LineId", Comparison)) { LineId = ChangeType<int?>(value); return; }
                if (fieldName.Equals("Line", Comparison)) { Line = ChangeType<string>(value); }
            }
        }
    }
}