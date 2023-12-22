using System.ComponentModel.DataAnnotations;

namespace legallead.logging.tests.testobj
{
    internal class ValidationDto
    {
        [Range(5, 15)]
        public int Index { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string Name { get; set; } = string.Empty;
    }
}