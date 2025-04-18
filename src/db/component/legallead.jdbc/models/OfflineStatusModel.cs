namespace legallead.jdbc.models
{
    public class OfflineStatusModel : OfflineRequestModel
    {
        public string LeadUserId { get; set; } = string.Empty;
        public string CountyName { get; set; } = string.Empty;
        public DateTime? SearchStartDate { get; set; }
        public DateTime? SearchEndDate { get; set; }
        public decimal? PercentComplete { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}
