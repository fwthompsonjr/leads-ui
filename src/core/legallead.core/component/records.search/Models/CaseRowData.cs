namespace legallead.records.search.Models
{
    public class CaseDataAddress
    {
        public string Case { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Party { get; set; } = string.Empty;
        public string Attorney { get; set; } = string.Empty;
    }

    public class CaseRowData
    {
        public int RowId { get; set; }
        public string Case { get; set; } = string.Empty;
        public string Court { get; set; } = string.Empty;
        public string FileDate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string TypeDesc { get; set; } = string.Empty;
        public string Subtype { get; set; } = string.Empty;
        public string Style { get; set; } = string.Empty;

        public IList<CaseDataAddress> CaseDataAddresses { get; set; } = new List<CaseDataAddress>();
    }
}