namespace legallead.permissions.api.Models
{
    public class UserSearchQueryModel
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? UserId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? EstimatedRowCount { get; set; }

        public DateTime? CreateDate { get; set; }

        public string? SearchProgress { get; set; }

        public string? StateCode { get; set; }

        public string? CountyName { get; set; }
    }
}
