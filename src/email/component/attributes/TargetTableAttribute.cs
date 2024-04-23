namespace legallead.email.attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TargetTableAttribute(string tableName) : Attribute
    {
        public string TableName { get; set; } = tableName;
    }
}
