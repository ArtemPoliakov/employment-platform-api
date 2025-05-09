using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Dtos.OfferDtos
{
    public class SetJobseekerReactionDto
    {
        [Required]
        public Guid JobseekerId { get; set; }

        [Required]
        public Guid VacancyId { get; set; }

        [Required]
        public JobApplicationStatuses Status { get; set; }

        [MinLength(1)]
        public string JobseekerResponse { get; set; } = string.Empty;
    }
}