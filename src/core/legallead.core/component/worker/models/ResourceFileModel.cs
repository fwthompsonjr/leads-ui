namespace legallead.reader.component.models
{
    public class ResourceFileModel
    {
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
    }
}
