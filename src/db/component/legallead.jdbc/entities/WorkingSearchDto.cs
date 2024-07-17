namespace legallead.jdbc.entities
{
    public class WorkingSearchDto : BaseDto
    {
        public string? SearchId { get; set; }
        public int MessageId { get; set; }
        public int StatusId { get; set; }
        public string? MachineName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdateDt { get; set; }
        public DateTime? CompletionDate { get; set; }
        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("SearchId", Comparison)) return SearchId;
                if (fieldName.Equals("MessageId", Comparison)) return MessageId;
                if (fieldName.Equals("StatusId", Comparison)) return StatusId;
                if (fieldName.Equals("MachineName", Comparison)) return MachineName;
                if (fieldName.Equals("CreateDate", Comparison)) return CreateDate;
                if (fieldName.Equals("LastUpdateDt", Comparison)) return LastUpdateDt;
                return CompletionDate;
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
                if (fieldName.Equals("SearchId", Comparison))
                {
                    SearchId = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("MessageId", Comparison))
                {
                    MessageId = ChangeType<int>(value);
                    return;
                }
                if (fieldName.Equals("StatusId", Comparison))
                {
                    StatusId = ChangeType<int>(value);
                    return;
                }
                if (fieldName.Equals("MachineName", Comparison))
                {
                    MachineName = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("CreateDate", Comparison))
                {
                    CreateDate = ChangeType<DateTime>(value);
                }
                if (fieldName.Equals("LastUpdateDt", Comparison))
                {
                    LastUpdateDt = ChangeType<DateTime>(value);
                }
                if (fieldName.Equals("CompletionDate", Comparison))
                {
                    CompletionDate = ChangeType<DateTime?>(value);
                }
            }
        }
    }
}
