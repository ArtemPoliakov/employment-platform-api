using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Dtos.JobApplicationDtos
{
    public class UpdateJobApplicationDto
    {
        [Required]
        public Guid JobseekerId { get; set; }

        [Required]
        public Guid VacancyId { get; set; }

        [MinLength(1)]
        public string? CompanyResponse { get; set; }

        public JobApplicationStatuses? Status { get; set; }
    }
}