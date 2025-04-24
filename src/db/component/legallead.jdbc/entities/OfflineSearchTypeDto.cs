namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "OFF_SEARCHTYPE_RSP")]
    public class OfflineSearchTypeDto : BaseDto
    {
        public int? ItemCount { get; set; }
        public string? SearchType { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("ItemCount", Comparison)) return ItemCount;
                return SearchType;
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
                if (fieldName.Equals("ItemCount", Comparison)) { ItemCount = ChangeType<int?>(value); return; }
                if (fieldName.Equals("SearchType", Comparison)) { SearchType = ChangeType<string?>(value); }
            }
        }
    }
}