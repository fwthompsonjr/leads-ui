namespace legallead.records.search.Classes
{
    public class DentonTableRead
    {
        public string? Heading { get; set; }
        public string? Records { get; set; }
        public DentonTableReadRecord[]? RecordSet { get; set; }
        public DentonTableReadHeader? Header { get; set; }
    }
}
