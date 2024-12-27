namespace legallead.jdbc.entities
{
    public class DbUsageSummaryBo
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string LeadUserId { get; set; } = string.Empty;
        public int SearchYear { get; set; }
        public int SearchMonth { get; set; }
        public DateTime? LastSearchDate { get; set; }
        public int CountyId { get; set; }
        public string CountyName { get; set; } = string.Empty;
        public int MTD { get; set; }
        public int MonthlyLimit { get; set; }
        public string? ExcelName { get; set; }
    }
}