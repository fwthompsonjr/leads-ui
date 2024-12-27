// AppendUsageRecordRequest
namespace legallead.permissions.api.Models
{
    public class AppendUsageRecordRequest
    {
        public string LeadUserId { get; set; } = string.Empty;
        public int CountyId { get; set; }
        public string CountyName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RecordCount { get; set; }
    }
}