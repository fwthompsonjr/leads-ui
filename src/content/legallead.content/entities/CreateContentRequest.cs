using System.ComponentModel.DataAnnotations;

namespace legallead.content.entities
{
    public class CreateContentRequest
    {
        [StringLength(500)]
        public string? Name { get; set; }
    }
}