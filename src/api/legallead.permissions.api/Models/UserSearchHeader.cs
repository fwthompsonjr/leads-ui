namespace legallead.permissions.api.Model
{
    public class UserSearchHeader
    {
        public string Id { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? EstimatedRowCount { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
