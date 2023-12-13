namespace legallead.content.attr
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DbTableAttribute : Attribute
    {
        public string TableName { get; set; } = string.Empty;
    }
}