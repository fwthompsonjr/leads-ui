namespace legallead.records.search
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataActionAttribute : Attribute
    {
        public int ProcessId { get; set; }
        public string Name { get; set; }

        public bool IsShared { get; set; }
    }
}