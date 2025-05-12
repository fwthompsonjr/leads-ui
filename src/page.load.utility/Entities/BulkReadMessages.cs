using page.load.utility.Interfaces;

namespace page.load.utility.Entities
{
    public class BulkReadMessages
    {
        public string OfflineRequestId { get; set; } = string.Empty;
        public List<string> Messages { get; set; } = [];
        public int RecordCount { get; set; }
        public int TotalProcessed { get; set; }
        public string Workload { get; set; } = string.Empty;
        public int RetryCount { get; set; }
        public IFetchDbAddress? AddressSerice { get; set; } = null;
    }
}
