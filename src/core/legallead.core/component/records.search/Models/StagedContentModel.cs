namespace legallead.records.search.Models
{
    public class StagedContentModel
    {
        public string? Id { get; set; }
        public string? SearchId { get; set; }
        public string? StagingType { get; set; }
        public int? LineNbr { get; set; }
        public string? Line { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
