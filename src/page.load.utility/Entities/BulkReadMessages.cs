namespace page.load.utility.Entities
{
    public class BulkReadMessages
    {
        public string OfflineRequestId { get; set; } = string.Empty;
        public List<string> Messages { get; set; } = [];
    }
}
