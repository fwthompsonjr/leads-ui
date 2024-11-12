namespace legallead.jdbc.entities
{
    public class LeadUserBo
    {
        public static int UserIndex => 0;
        public static int CountyIndex => 1;
        public static int DataIndex => 2;
        public string Id { get; set; } = string.Empty;
        public List<string> Keys { get; } = ["", "", ""];
        public string UserName { get; set; } = string.Empty;
        public string UserData { get; set; } = string.Empty;
        public string CountyData { get; set; } = string.Empty;
        public string IndexData { get; set; } = string.Empty;
    }
}
