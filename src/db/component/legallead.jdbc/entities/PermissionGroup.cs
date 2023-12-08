namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "permissiongroup")]
    public class PermissionGroup : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public int? GroupId { get; set; }
        public int? OrderId { get; set; }
        public int? PerRequest { get; set; }
        public int? PerMonth { get; set; }
        public int? PerYear { get; set; }
        public bool? IsActive { get; set; } = true;
        public bool? IsVisible { get; set; } = true;
        public DateTime? CreateDate { get; set; } = DateTime.UtcNow;

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("Name", Comparison)) return Name;
                if (fieldName.Equals("GroupId", Comparison)) return GroupId;
                if (fieldName.Equals("OrderId", Comparison)) return OrderId;
                if (fieldName.Equals("PerRequest", Comparison)) return PerRequest;
                if (fieldName.Equals("PerMonth", Comparison)) return PerMonth;
                if (fieldName.Equals("PerYear", Comparison)) return PerYear;
                if (fieldName.Equals("IsActive", Comparison)) return IsActive;
                if (fieldName.Equals("IsVisible", Comparison)) return IsVisible;
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
                    Id = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("Name", Comparison))
                {
                    Name = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("GroupId", Comparison))
                {
                    GroupId = ChangeType<int?>(value);
                    return;
                }
                if (fieldName.Equals("OrderId", Comparison))
                {
                    OrderId = ChangeType<int?>(value);
                    return;
                }
                if (fieldName.Equals("PerRequest", Comparison))
                {
                    PerRequest = ChangeType<int?>(value);
                    return;
                }
                if (fieldName.Equals("PerMonth", Comparison))
                {
                    PerMonth = ChangeType<int?>(value);
                    return;
                }
                if (fieldName.Equals("PerYear", Comparison))
                {
                    PerYear = ChangeType<int?>(value);
                    return;
                }
                if (fieldName.Equals("IsActive", Comparison))
                {
                    IsActive = ChangeType<bool?>(value);
                    return;
                }
                if (fieldName.Equals("IsVisible", Comparison))
                {
                    IsVisible = ChangeType<bool?>(value);
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