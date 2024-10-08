﻿using System.ComponentModel.DataAnnotations;

namespace legallead.models.Search
{
    public class SearchRequest
    {
        public int WebId { get; set; }
        [Required]
        public string State { get; set; } = string.Empty;
        [Required]
        public string County { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Date)]
        public string StartDate { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Date)]
        public string EndDate { get; set; } = string.Empty;
    }
}
