using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VacancyDtos;
using api.Enums;

namespace api.Dtos.JobApplicationDtos
{
    public class JobApplicationWithVacancyDto
    {
        public VacancyCompactDto VacancyCompactDto { get; set; }

        public Guid JobseekerId { get; set; }
        public Guid VacancyId { get; set; }
        public string Status { get; set; }
        public string CompanyResponse { get; set; }
        public DateTime CreationDate { get; set; }
    }
}