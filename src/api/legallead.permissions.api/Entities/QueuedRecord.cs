namespace legallead.permissions.api.Entities
{
    public class QueuedRecord
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? UserId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? ExpectedRows { get; set; }

        public DateTime? CreateDate { get; set; }
        public string? Payload { get; set; }
    }
}
