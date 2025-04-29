using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Dtos.VacancyDtos
{
    public class EditVacancyDto
    {
        [MinLength(3, ErrorMessage = "Vacancy title must be at least 3 characters long")]
        [MaxLength(60, ErrorMessage = "Vacancy title must be less than 60 characters long")]
        public string? Title { get; set; }

        [MinLength(3, ErrorMessage = "Vacancy description must be at least 3 characters long")]
        [MaxLength(700, ErrorMessage = "Vacancy description must be less than 700 characters long")]
        public string? Description { get; set; }

        [MinLength(3, ErrorMessage = "Candidate description must be at least 3 characters long")]
        [MaxLength(700, ErrorMessage = "Candidate description must be less than 700 characters long")]
        public string? CandidateDescription { get; set; }

        [MinLength(1, ErrorMessage = "Position must be at least 1 character long")]
        [MaxLength(60, ErrorMessage = "Position must be less than 60 characters long")]
        public string? Position { get; set; }

        [Range(1, 100_000_000_000, ErrorMessage = "Min salary must be greater than 0 and less or equal to 100_000_000_000")]
        public float? SalaryMin { get; set; }

        [Range(1, 100_000_000_000, ErrorMessage = "Max salary must be greater than 0 and less or equal to 100_000_000_000")]
        public float? SalaryMax { get; set; }

        public VacancyWorkModes? WorkMode { get; set; }

        [MinLength(3, ErrorMessage = "Living conditions must be at least 3 characters long")]
        [MaxLength(400, ErrorMessage = "Living conditions must be less than 400 characters long")]
        public string? LivingConditions { get; set; }
    }
}