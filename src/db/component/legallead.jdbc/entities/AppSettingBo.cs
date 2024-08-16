namespace legallead.jdbc.entities
{
    public class AppSettingBo
    {
        public string? Id { get; set; }
        public string? KeyName { get; set; }
        public string? KeyValue { get; set; }
        public decimal? Version { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}