using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Entities
{
    public class QueueSummaryRequest : BaseQueueRequest
    {
        [Required]
        [Range(0, 2)]
        public int? StatusId { get; set; }
    }
}