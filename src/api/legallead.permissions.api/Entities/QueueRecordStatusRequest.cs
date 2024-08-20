namespace legallead.permissions.api.Entities
{
    public class QueueRecordStatusRequest : BaseQueueRequest
    {
        public string UniqueId { get; set; } = string.Empty;
        public int? MessageId { get; set; }
        public int? StatusId { get; set; }
    }
}
