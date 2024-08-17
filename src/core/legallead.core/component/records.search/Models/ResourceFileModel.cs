namespace legallead.records.search.Models
{
    internal class ResourceFileModel
    {
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
    }
}
