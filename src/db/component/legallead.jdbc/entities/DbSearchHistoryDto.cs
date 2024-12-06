namespace legallead.jdbc.entities
{
    public class DbSearchHistoryDto : BaseDto
    {
        public int CountyId { get; set; }
        public int RecordCount { get; set; }
        public DateTime SearchDate { get; set; }
        public int SearchTypeId { get; set; }
        public int CaseTypeId { get; set; }
        public int DistrictCourtId { get; set; }
        public int DistrictSearchTypeId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? CompleteDate { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("CountyId", Comparison)) return CountyId;
                if (fieldName.Equals("RecordCount", Comparison)) return RecordCount;
                if (fieldName.Equals("SearchDate", Comparison)) return SearchDate;
                if (fieldName.Equals("SearchTypeId", Comparison)) return SearchTypeId;
                if (fieldName.Equals("CaseTypeId", Comparison)) return CaseTypeId;
                if (fieldName.Equals("DistrictCourtId", Comparison)) return DistrictCourtId;
                if (fieldName.Equals("DistrictSearchTypeId", Comparison)) return DistrictSearchTypeId;
                if (fieldName.Equals("CreateDate", Comparison)) return CreateDate;
                return CompleteDate;
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
                if (fieldName.Equals("CountyId", Comparison)) { CountyId = ChangeType<int>(value); return; }
                if (fieldName.Equals("RecordCount", Comparison)) { RecordCount = ChangeType<int>(value); return; }
                if (fieldName.Equals("SearchDate", Comparison)) { SearchDate = ChangeType<DateTime>(value); return; }
                if (fieldName.Equals("SearchTypeId", Comparison)) { SearchTypeId = ChangeType<int>(value); return; }
                if (fieldName.Equals("CaseTypeId", Comparison)) { CaseTypeId = ChangeType<int>(value); return; }
                if (fieldName.Equals("DistrictCourtId", Comparison)) { DistrictCourtId = ChangeType<int>(value); return; }
                if (fieldName.Equals("DistrictSearchTypeId", Comparison)) { DistrictSearchTypeId = ChangeType<int>(value); return; }
                if (fieldName.Equals("CreateDate", Comparison)) { CreateDate = ChangeType<DateTime?>(value); }
            }
        }

    }
}
