using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.OfferDtos
{
    public class EditCompanyMessageDto
    {
        [Required]
        public Guid JobseekerId { get; set; }

        [Required]
        public Guid VacancyId { get; set; }

        [MinLength(1)]
        [Required]
        public string CompanyMessage { get; set; } = string.Empty;
    }
}