namespace legallead.jdbc.entities
{
    public class DbCountyUsageRequestBo
    {
        public string Id { get; set; } = string.Empty;
        public string LeadUserId { get; set; } = string.Empty;
        public int CountyId { get; set; }
        public string CountyName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DateRange { get; set; } = string.Empty;
        public int RecordCount { get; set; }
        public DateTime? CompleteDate { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}