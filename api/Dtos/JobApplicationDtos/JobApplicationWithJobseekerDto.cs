using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.JobseekerDto;

namespace api.Dtos.JobApplicationDtos
{
    public class JobApplicationWithJobseekerDto
    {
        public JobseekerCompactSearchResultDto JobseekerCompactDto { get; set; }
        public Guid JobseekerId { get; set; }
        public Guid VacancyId { get; set; }
        public string Status { get; set; }
        public string CompanyResponse { get; set; }
        public DateTime CreationDate { get; set; }
    }
}