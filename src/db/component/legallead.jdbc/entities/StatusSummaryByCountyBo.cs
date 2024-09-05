namespace legallead.jdbc.entities
{
    public class StatusSummaryByCountyBo
    {
        public string? Region { get; set; }
        public int? Count { get; set; }
        public DateTime? Oldest { get; set; }
        public DateTime? Newest { get; set; }
    }
}