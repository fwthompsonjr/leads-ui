namespace legallead.jdbc
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TargetTableAttribute : Attribute
    {
        public string TableName { get; set; } = string.Empty;
    }
}