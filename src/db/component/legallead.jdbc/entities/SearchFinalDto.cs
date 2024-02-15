namespace legallead.jdbc.entities
{
    public class SearchFinalDto : BaseDto
    {
        public string SearchId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Address3 { get; set; } = string.Empty;
        public string CaseNumber { get; set; } = string.Empty;
        public string DateFiled { get; set; } = string.Empty;
        public string Court { get; set; } = string.Empty;
        public string CaseType { get; set; } = string.Empty;
        public string CaseStyle { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Plantiff { get; set; } = string.Empty;
        public string County { get; set; } = string.Empty;
        public string CourtAddress { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

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
                if (fieldName.Equals("County", Comparison)) return County;
                if (fieldName.Equals("CourtAddress", Comparison)) return CourtAddress;
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
                if (fieldName.Equals("SearchId", Comparison)) { SearchId = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("Name", Comparison)) { Name = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("Zip", Comparison)) { Zip = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("Address1", Comparison)) { Address1 = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("Address2", Comparison)) { Address2 = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("Address3", Comparison)) { Address3 = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("CaseNumber", Comparison)) { CaseNumber = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("DateFiled", Comparison)) { DateFiled = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("Court", Comparison)) { Court = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("CaseType", Comparison)) { CaseType = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("CaseStyle", Comparison)) { CaseStyle = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("FirstName", Comparison)) { FirstName = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("LastName", Comparison)) { LastName = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("Plantiff", Comparison)) { Plantiff = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("County", Comparison)) { County = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("CourtAddress", Comparison)) { CourtAddress = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("Status", Comparison)) { Status = ChangeType<string>(value) ?? string.Empty; }
            }
        }
    }
}
