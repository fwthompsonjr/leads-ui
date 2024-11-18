namespace legallead.permissions.api.Entities
{
    public class GetUsageUserByIdResponse
    {

        public string? CountyName { get; set; }
        public int MonthlyUsage { get; set; }
        public DateTime? CreateDate { get; set; }

        public int IncidentMonth => CreateDate.GetValueOrDefault().Month;
        public int IncidentYear => CreateDate.GetValueOrDefault().Year;
    }
}
