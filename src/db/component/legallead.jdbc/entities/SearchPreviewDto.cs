namespace legallead.jdbc.entities
{
    public class SearchPreviewDto : BaseDto
    {
        public string? SearchId { get; set; }
        public string? Name { get; set; }
        public string? Zip { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? CaseNumber { get; set; }
        public string? DateFiled { get; set; }
        public string? Court { get; set; }
        public string? CaseType { get; set; }
        public string? CaseStyle { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Plantiff { get; set; }
        public string? Status { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("SearchId", Comparison)) return SearchId;
                if (fieldName.Equals("Name", Comparison)) return Name;
                if (fieldName.Equals("Zip", Comparison)) return Zip;
                if (fieldName.Equals("Address1", Comparison)) return Address1;
                if (fieldName.Equals("Address2", Comparison)) return Address2;
                if (fieldName.Equals("Address3", Comparison)) return Address3;
                if (fieldName.Equals("CaseNumber", Comparison)) return CaseNumber;
                if (fieldName.Equals("DateFiled", Comparison)) return DateFiled;
                if (fieldName.Equals("Court", Comparison)) return Court;
                if (fieldName.Equals("CaseType", Comparison)) return CaseType;
                if (fieldName.Equals("CaseStyle", Comparison)) return CaseStyle;
                if (fieldName.Equals("FirstName", Comparison)) return FirstName;
                if (fieldName.Equals("LastName", Comparison)) return LastName;
                if (fieldName.Equals("Plantiff", Comparison)) return Plantiff;
                if (fieldName.Equals("Status", Comparison)) return Status;
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
                if (fieldName.Equals("SearchId", Comparison)) SearchId = ChangeType<string>(value);
                if (fieldName.Equals("Name", Comparison)) Name = ChangeType<string>(value);
                if (fieldName.Equals("Zip", Comparison)) Zip = ChangeType<string>(value);
                if (fieldName.Equals("Address1", Comparison)) Address1 = ChangeType<string>(value);
                if (fieldName.Equals("Address2", Comparison)) Address2 = ChangeType<string>(value);
                if (fieldName.Equals("Address3", Comparison)) Address3 = ChangeType<string>(value);
                if (fieldName.Equals("CaseNumber", Comparison)) CaseNumber = ChangeType<string>(value);
                if (fieldName.Equals("DateFiled", Comparison)) DateFiled = ChangeType<string>(value);
                if (fieldName.Equals("Court", Comparison)) Court = ChangeType<string>(value);
                if (fieldName.Equals("CaseType", Comparison)) CaseType = ChangeType<string>(value);
                if (fieldName.Equals("CaseStyle", Comparison)) CaseStyle = ChangeType<string>(value);
                if (fieldName.Equals("FirstName", Comparison)) FirstName = ChangeType<string>(value);
                if (fieldName.Equals("LastName", Comparison)) LastName = ChangeType<string>(value);
                if (fieldName.Equals("Plantiff", Comparison)) Plantiff = ChangeType<string>(value);
                if (fieldName.Equals("Status", Comparison)) Status = ChangeType<string>(value);
            }
        }
    }
}
