namespace legallead.jdbc.entities
{
    public class LeadCountyPricingBo
    {
        public string? Id { get; set; }
        public int? CountyId { get; set; }
        public string? CountyName { get; set; }
        public bool? IsActive { get; set; }
        public decimal? PerRecord { get; set; }
        public DateTime? CompleteDate { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}