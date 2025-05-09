using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.OfferDtos
{
    public class CreateOfferDto
    {
        [Required]
        public string JobseekerUsername { get; set; } = string.Empty;

        [Required]
        public Guid VacancyId { get; set; }

        [MinLength(1)]
        public string CompanyMessage { get; set; } = string.Empty;
    }
}