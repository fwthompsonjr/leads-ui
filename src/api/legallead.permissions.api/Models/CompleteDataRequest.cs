using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Models
{
    public class CompleteDataRequest
    {
        [Range(0, 1000)]
        public int CountyId { get; set; }
        public DateTime SearchDate { get; set; }
        [Range(0, 50)]
        public int SearchTypeId { get; set; }
        [Range(0, 50)]
        public int CaseTypeId { get; set; }
        [Range(0, 50)]
        public int DistrictCourtId { get; set; }
        [Range(0, 50)]
        public int DistrictSearchTypeId { get; set; }
    }
}
