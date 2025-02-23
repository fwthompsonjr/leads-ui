namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "DBCOUNTYFILE")]
    public class DbCountyFileDto : BaseDto
    {
        public string? DbCountyFileId { get; set; }
        public int? FileTypeId { get; set; }
        public int? FileStatusId { get; set; }
        public string? Content { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("DbCountyFileId", Comparison)) return DbCountyFileId;
                if (fieldName.Equals("FileTypeId", Comparison)) return FileTypeId;
                if (fieldName.Equals("FileStatusId", Comparison)) return FileStatusId;
                return Content;
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
                if (fieldName.Equals("DbCountyFileId", Comparison)) { DbCountyFileId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("FileTypeId", Comparison)) { FileTypeId = ChangeType<int?>(value); return; }
                if (fieldName.Equals("FileStatusId", Comparison)) { FileStatusId = ChangeType<int?>(value); return; }
                if (fieldName.Equals("Content", Comparison)) { Content = ChangeType<string?>(value); }
            }
        }

    }
}