namespace legallead.logging
{
    internal class LogInsertModel
    {
        public string? RequestId { get; set; }
        public int? StatusId { get; set; }
        public int? LineNumber { get; set; }
        public string? NameSpace { get; set; }
        public string? ClassName { get; set; }
        public string? MethodName { get; set; }
        public string? Message { get; set; }
        public string? Detail { get; set; }
    }
}