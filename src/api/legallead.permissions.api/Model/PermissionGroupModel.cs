namespace legallead.permissions.api.Model
{
    public class PermissionGroupModel
    {
        public string Name { get; set; } = string.Empty;
        public int? GroupId { get; set; }
        public int? OrderId { get; set; }
        public int? PerRequest { get; set; }
        public int? PerMonth { get; set; }
        public int? PerYear { get; set; }
        public bool? IsActive { get; set; }
    }
}
