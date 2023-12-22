using legallead.logging.attr;

namespace legallead.logging.entities
{
    [LogTable(TableName = "VWLOG")]
    public class VwLogDto : CommonBaseDto
    {
        public string? RequestId { get; set; }
        public int? StatusId { get; set; }
        public int? LineNumber { get; set; }
        public string? NameSpace { get; set; }
        public string? ClassName { get; set; }
        public string? MethodName { get; set; }
        public string? Message { get; set; }
        public int? LineId { get; set; }
        public string? Line { get; set; }
        public DateTime? CreateDate { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("RequestId", Comparison)) return RequestId;
                if (fieldName.Equals("LineNumber", Comparison)) return LineNumber;
                if (fieldName.Equals("NameSpace", Comparison)) return NameSpace;
                if (fieldName.Equals("ClassName", Comparison)) return ClassName;
                if (fieldName.Equals("MethodName", Comparison)) return MethodName;
                if (fieldName.Equals("Message", Comparison)) return Message;
                if (fieldName.Equals("LineId", Comparison)) return LineId;
                if (fieldName.Equals("Line", Comparison)) return Line;
                if (fieldName.Equals("CreateDate", Comparison)) return CreateDate;
                return null;
            }
            set
            {
                if (field == null) return;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return;
                if (fieldName.Equals("Id", Comparison)) { Id = ChangeType<long?>(value); return; }
                if (fieldName.Equals("RequestId", Comparison)) { RequestId = ChangeType<string>(value); return; }
                if (fieldName.Equals("StatusId", Comparison)) { StatusId = ChangeType<int?>(value); return; }
                if (fieldName.Equals("LineNumber", Comparison)) { LineNumber = ChangeType<int?>(value); return; }
                if (fieldName.Equals("NameSpace", Comparison)) { NameSpace = ChangeType<string>(value); return; }
                if (fieldName.Equals("ClassName", Comparison)) { ClassName = ChangeType<string>(value); return; }
                if (fieldName.Equals("MethodName", Comparison)) { MethodName = ChangeType<string>(value); return; }
                if (fieldName.Equals("Message", Comparison)) { Message = ChangeType<string>(value); return; }
                if (fieldName.Equals("LineId", Comparison)) { LineId = ChangeType<int?>(value); return; }
                if (fieldName.Equals("Line", Comparison)) { Line = ChangeType<string>(value); return; }
                if (fieldName.Equals("CreateDate", Comparison)) { CreateDate = ChangeType<DateTime?>(value); }
            }
        }
    }
}