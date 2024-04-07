namespace legallead.permissions.api.Model
{
    public class UserSearchDetail
    {
        public string Context { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public string Line { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
    }
}
