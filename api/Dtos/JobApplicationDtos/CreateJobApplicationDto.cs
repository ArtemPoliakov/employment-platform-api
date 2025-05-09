using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.JobApplicationDtos
{
    public class CreateJobApplicationDto
    {
        [Required]
        public string JobseekerUsername { get; set; } = string.Empty;

        [Required]
        public Guid VacancyId { get; set; }
    }
}