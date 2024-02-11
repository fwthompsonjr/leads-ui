namespace legallead.jdbc.entities
{
    public class SearchInvoiceBo
    {
        public string? LineId { get; set; }
        public string? UserId { get; set; }
        public string? ItemType { get; set; }
        public int? ItemCount { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Price { get; set; }
        public string? ReferenceId { get; set; }
        public string? ExternalId { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
