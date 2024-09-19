namespace legallead.jdbc.entities
{
    public class QueueNonPersonBo
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public int? ExpectedRows { get; set; }
        public string? SearchProgress { get; set; }
        public string? StateCode { get; set; }
        public string? CountyName { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public byte[]? ExcelData { get; set; }
    }
}