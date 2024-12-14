namespace legallead.jdbc.entities
{
    public class DbCountyUsageLimitBo
    {
        public string Id { get; set; } = string.Empty;
        public string LeadUserId { get; set; } = string.Empty;
        public int CountyId { get; set; }
        public bool IsActive { get; set; }
        public int MaxRecords { get; set; }
        public DateTime? CompleteDate { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}