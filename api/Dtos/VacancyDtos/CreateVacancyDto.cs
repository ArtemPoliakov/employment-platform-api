using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Dtos.VacancyDtos
{
    /// <summary>
    /// Dto for vacancy creation
    /// </summary>
    public class CreateVacancyDto
    {
        [MinLength(3, ErrorMessage = "Vacancy title must be at least 3 characters long")]
        [MaxLength(60, ErrorMessage = "Vacancy title must be less than 60 characters long")]
        [Required]
        public string Title { get; set; } = string.Empty;

        [MinLength(3, ErrorMessage = "Vacancy description must be at least 3 characters long")]
        [MaxLength(700, ErrorMessage = "Vacancy description must be less than 700 characters long")]
        [Required]
        public string Description { get; set; } = string.Empty;

        [MinLength(3, ErrorMessage = "Candidate description must be at least 3 characters long")]
        [MaxLength(700, ErrorMessage = "Candidate description must be less than 700 characters long")]
        [Required]
        public string CandidateDescription { get; set; } = string.Empty;

        [MinLength(1, ErrorMessage = "Position must be at least 1 character long")]
        [MaxLength(60, ErrorMessage = "Position must be less than 60 characters long")]
        [Required]
        public string Position { get; set; } = string.Empty;

        [Range(1, 100_000_000_000, ErrorMessage = "Min salary must be greater than 0 and less or equal to 100_000_000_000")]
        [Required]
        public float SalaryMin { get; set; } = 0;

        [Range(1, 100_000_000_000, ErrorMessage = "Max salary must be greater than 0 and less or equal to 100_000_000_000")]
        [Required]
        public float SalaryMax { get; set; } = 100_000_000_000;

        [Required]
        public VacancyWorkModes WorkMode { get; set; } = VacancyWorkModes.NONE;

        [MinLength(3, ErrorMessage = "Living conditions must be at least 3 characters long")]
        [MaxLength(400, ErrorMessage = "Living conditions must be less than 400 characters long")]
        [Required]
        public string LivingConditions { get; set; } = string.Empty;
    }
}