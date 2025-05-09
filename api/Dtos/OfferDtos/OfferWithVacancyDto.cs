using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VacancyDtos;

namespace api.Dtos.OfferDtos
{
    public class OfferWithVacancyDto
    {
        public VacancyCompactDto VacancyCompactDto { get; set; }

        public Guid JobseekerId { get; set; }
        public Guid VacancyId { get; set; }
        public string Status { get; set; }
        public string CompanyMessage { get; set; }
        public string JobseekerResponse { get; set; }
        public DateTime CreationDate { get; set; }
    }
}