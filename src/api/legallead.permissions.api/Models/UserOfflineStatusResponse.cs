namespace legallead.permissions.api.Models
{
    public class UserOfflineStatusResponse : OfflineDataModel
    {
        public bool? IsCompleted { get; set; }

        public string LeadUserId { get; set; } = string.Empty;


        public string CountyName { get; set; } = string.Empty;


        public DateTime? SearchStartDate { get; set; }

        public DateTime? SearchEndDate { get; set; }

        public decimal? PercentComplete { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? LastUpdate { get; set; }
    }
}
