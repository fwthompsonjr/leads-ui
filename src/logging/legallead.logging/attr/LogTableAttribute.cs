namespace legallead.logging.attr
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LogTableAttribute : Attribute
    {
        public string TableName { get; set; } = string.Empty;
    }
}