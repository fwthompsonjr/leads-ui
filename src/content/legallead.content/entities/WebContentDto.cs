using legallead.content.attr;

namespace legallead.content.entities
{
    [DbTable(TableName = "Content")]
    public class WebContentDto : CommonBaseDto
    {
        public int? InternalId { get; set; }
        public int? VersionId { get; set; }
        public string? ContentName { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsChild { get; set; } = false;
        public DateTime? CreateDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("VersionId", Comparison)) return VersionId;
                if (fieldName.Equals("InternalId", Comparison)) return InternalId;
                if (fieldName.Equals("ContentName", Comparison)) return ContentName;
                if (fieldName.Equals("IsActive", Comparison)) return IsActive;
                if (fieldName.Equals("IsChild", Comparison)) return IsChild;
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
                if (fieldName.Equals("InternalId", Comparison))
                {
                    InternalId = ChangeType<int?>(value);
                    return;
                }
                if (fieldName.Equals("VersionId", Comparison))
                {
                    VersionId = ChangeType<int?>(value);
                    return;
                }
                if (fieldName.Equals("ContentName", Comparison))
                {
                    ContentName = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("IsActive", Comparison))
                {
                    IsActive = ChangeType<bool?>(value);
                    return;
                }
                if (fieldName.Equals("IsChild", Comparison))
                {
                    IsChild = ChangeType<bool?>(value);
                    return;
                }
                if (fieldName.Equals("CreateDate", Comparison))
                {
                    CreateDate = ChangeType<DateTime?>(value);
                }
            }
        }
    }
}