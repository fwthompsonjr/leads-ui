namespace legallead.jdbc.entities
{
    public class QueueWorkingBo
    {
        public string? Id { get; set; }
        public string? SearchId { get; set; }
        public string? Message { get; set; }
        public int? StatusId { get; set; }
        public string? MachineName { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastUpdateDt { get; set; }
        public DateTime? CompletionDate { get; set; }
    }
}
