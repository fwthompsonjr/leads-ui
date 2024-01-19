namespace legallead.logging
{
    public class LogQueryModel
    {
        public long? Id { get; set; }
        public string? RequestId { get; set; }
        public int? StatusId { get; set; }
        public string? NameSpace { get; set; }
        public string? ClassName { get; set; }
        public string? MethodName { get; set; }
    }
}