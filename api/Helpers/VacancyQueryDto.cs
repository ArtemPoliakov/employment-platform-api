using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Helpers
{
    public class VacancyQueryDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;

        [MinLength(1, ErrorMessage = "Position must be at least 1 character")]
        [MaxLength(100, ErrorMessage = "Position must be at most 100 characters")]
        public string? Position { get; set; }

        [Range(1, 100_000_000_000, ErrorMessage = "Min salary must be greater than 0 and less or equal to 100_000_000_000")]
        public float MinSalary { get; set; } = 1;

        [Range(1, 100_000_000_000, ErrorMessage = "Max salary must be greater than 0 and less or equal to 100_000_000_000")]
        public float MaxSalary { get; set; } = 100_000_000_000;
        public string WorkMode { get; set; } = string.Empty;

        [MinLength(1, ErrorMessage = "GeneralDescription must be at least 1 character")]
        [MaxLength(300, ErrorMessage = "GeneralDescription must be at most 300 characters")]
        public string? GeneralDescription { get; set; }
    }
}