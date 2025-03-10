namespace legallead.jdbc.entities
{
    public class LeadUserAccountBo
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool? IsAdministrator { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}